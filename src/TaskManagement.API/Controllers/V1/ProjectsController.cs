using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskManagement.Application.Common;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Application.Features.Projects.DTOs;
using TaskManagement.Application.Features.Projects.Queries.GetAllProjects;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;

namespace TaskManagement.API.Controllers.V1;

[ApiController]
[Authorize]
[Route("api/v1/projects")]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProjectDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllProjectsQuery(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ProjectDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<ProjectDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new CreateProjectCommand(request.Name, request.Description),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = dto.Id },
            ApiResponse<ProjectDto>.Ok(dto, "Project created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProjectDto>>> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new UpdateProjectCommand(id, request.Name, request.Description),
            cancellationToken);

        return Ok(ApiResponse<ProjectDto>.Ok(dto, "Project updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProjectCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(null!, "Project deleted."));
    }
}
