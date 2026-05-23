using MediatR;

using TaskManagement.Application.Features.Tasks.DTOs;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed record GetTasksByProjectQuery(Guid ProjectId) : IRequest<IReadOnlyList<TaskItemDto>>;
