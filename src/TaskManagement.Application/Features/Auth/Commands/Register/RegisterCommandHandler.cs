using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Auth.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await db.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (exists)
            throw new ConflictException($"A user with email '{request.Email}' already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            FullName = request.FullName,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, user.FullName, token);
    }
}
