namespace TaskManagement.Application.Features.Projects.DTOs;

public sealed record UpdateProjectRequest(
    string Name,
    string? Description);
