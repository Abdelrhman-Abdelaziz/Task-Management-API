using MediatR;

using TaskManagement.Application.Features.Projects.DTOs;

namespace TaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;
