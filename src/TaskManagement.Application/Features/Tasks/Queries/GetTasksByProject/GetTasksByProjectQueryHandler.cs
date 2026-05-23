using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public sealed class GetTasksByProjectQueryHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetTasksByProjectQuery, IReadOnlyList<TaskItemDto>>
{
    public async Task<IReadOnlyList<TaskItemDto>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        var projectExists = await db.Projects
            .AnyAsync(p => p.Id == request.ProjectId && p.UserId == currentUser.UserId,
                cancellationToken);

        if (!projectExists)
            throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId);

        return await db.Tasks
            .Where(t => t.ProjectId == request.ProjectId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskItemDto(
                t.Id, t.Title, t.Description, t.Status,
                t.DueDate, t.Priority, t.ProjectId, t.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
