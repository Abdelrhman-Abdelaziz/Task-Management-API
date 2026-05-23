using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await db.Projects
            .Where(p => p.Id == request.Id && p.UserId == currentUser.UserId)
            .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        return project;
    }
}
