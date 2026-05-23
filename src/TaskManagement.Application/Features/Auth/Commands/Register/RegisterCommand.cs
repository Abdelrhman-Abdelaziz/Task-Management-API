using MediatR;

using TaskManagement.Application.Features.Auth.DTOs;

namespace TaskManagement.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FullName) : IRequest<AuthResponse>;
