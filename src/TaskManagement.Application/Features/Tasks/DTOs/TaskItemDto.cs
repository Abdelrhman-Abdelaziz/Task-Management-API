using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.DTOs;

public sealed record TaskItemDto(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid ProjectId,
    DateTime CreatedAt);
