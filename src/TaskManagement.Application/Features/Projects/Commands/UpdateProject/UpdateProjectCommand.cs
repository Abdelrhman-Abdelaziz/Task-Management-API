using MediatR;

using TaskManagement.Application.Features.Projects.DTOs;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description) : IRequest<ProjectDto>;
