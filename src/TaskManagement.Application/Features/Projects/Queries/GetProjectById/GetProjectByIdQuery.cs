using MediatR;

using TaskManagement.Application.Features.Projects.DTOs;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;
