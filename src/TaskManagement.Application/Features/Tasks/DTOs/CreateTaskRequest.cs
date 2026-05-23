using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.DTOs;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskPriority Priority);
