using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskManagement.Application.Common;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Application.Features.Tasks.DTOs;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

namespace TaskManagement.API.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/tasks")]
public sealed class TasksController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TaskItemDto>>>> GetByProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTasksByProjectQuery(projectId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TaskItemDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemDto>>> Create(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new CreateTaskCommand(projectId, request.Title, request.Description,
                request.DueDate, request.Priority),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetByProject),
            new { projectId, version = "1.0" },
            ApiResponse<TaskItemDto>.Ok(dto, "Task created."));
    }

    [HttpPatch("{taskId:guid}/status")]
    public async Task<ActionResult<ApiResponse<TaskItemDto>>> UpdateStatus(
        Guid taskId,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await sender.Send(
            new UpdateTaskStatusCommand(taskId, request.Status),
            cancellationToken);

        return Ok(ApiResponse<TaskItemDto>.Ok(dto, "Task status updated."));
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteTaskCommand(taskId), cancellationToken);
        return Ok(ApiResponse<object>.Ok(null!, "Task deleted."));
    }
}
