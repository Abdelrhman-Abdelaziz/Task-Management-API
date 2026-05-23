using MediatR;

using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public sealed record UpdateTaskStatusCommand(
    Guid Id,
    TaskItemStatus Status) : IRequest<TaskItemDto>;
