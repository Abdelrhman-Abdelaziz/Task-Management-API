using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteTaskCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.Project.UserId == currentUser.UserId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.Id);

        db.Tasks.Remove(task);
        await db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
