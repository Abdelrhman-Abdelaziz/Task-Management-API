using MediatR;

using TaskManagement.Application.Features.Auth.DTOs;

namespace TaskManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;
