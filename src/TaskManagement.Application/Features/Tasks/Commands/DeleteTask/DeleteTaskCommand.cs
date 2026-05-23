using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id) : IRequest<Unit>;
