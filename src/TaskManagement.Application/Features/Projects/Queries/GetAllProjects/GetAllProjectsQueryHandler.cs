using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public sealed class GetAllProjectsQueryHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>
{
    public async Task<IReadOnlyList<ProjectDto>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        return await db.Projects
            .Where(p => p.UserId == currentUser.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
