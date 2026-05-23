using MediatR;

using Microsoft.EntityFrameworkCore;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await db.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == currentUser.UserId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        project.Name = request.Name;
        project.Description = request.Description;

        await db.SaveChangesAsync(cancellationToken);

        return new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt);
    }
}
