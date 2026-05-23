using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public sealed class UpdateTaskStatusCommandHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateTaskStatusCommand, TaskItemDto>
{
    public async Task<TaskItemDto> Handle(
        UpdateTaskStatusCommand request,
        CancellationToken cancellationToken)
    {
        var task = await db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.Project.UserId == currentUser.UserId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        task.Status = request.Status;
        await db.SaveChangesAsync(cancellationToken);

        return new TaskItemDto(
            task.Id, task.Title, task.Description, task.Status,
            task.DueDate, task.Priority, task.ProjectId, task.CreatedAt);
    }
}
