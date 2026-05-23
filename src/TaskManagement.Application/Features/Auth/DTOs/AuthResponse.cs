namespace TaskManagement.Application.Features.Auth.DTOs;

public sealed record AuthResponse(
    Guid Id,
    string Email,
    string FullName,
    string Token);
