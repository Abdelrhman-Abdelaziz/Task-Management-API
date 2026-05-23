using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.DTOs;

public sealed record UpdateTaskStatusRequest(TaskItemStatus Status);
