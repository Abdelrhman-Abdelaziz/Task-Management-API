using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteProjectCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await db.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == currentUser.UserId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        db.Projects.Remove(project);
        await db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
