using MediatR;

using TaskManagement.Application.Features.Projects.DTOs;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string? Description) : IRequest<ProjectDto>;
