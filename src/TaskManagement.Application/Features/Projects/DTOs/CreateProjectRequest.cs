namespace TaskManagement.Application.Features.Projects.DTOs;

public sealed record CreateProjectRequest(
    string Name,
    string? Description);
