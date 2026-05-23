using MediatR;

using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskPriority Priority) : IRequest<TaskItemDto>;
