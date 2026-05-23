using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Tasks;

public sealed class UpdateTaskStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTaskNotOwned_ThrowsNotFoundException()
    {
        var db = TestDbContextFactory.Create();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Project",
            Description = "Desc",
            UserId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task",
            Description = "Desc",
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = otherUserId };
        var handler = new UpdateTaskStatusCommandHandler(db, currentUser);
        var command = new UpdateTaskStatusCommand(task.Id, TaskItemStatus.Done);

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_UpdatesStatus()
    {
        var db = TestDbContextFactory.Create();
        var ownerId = Guid.NewGuid();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Project",
            Description = "Desc",
            UserId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task",
            Description = "Desc",
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = ownerId };
        var handler = new UpdateTaskStatusCommandHandler(db, currentUser);
        var command = new UpdateTaskStatusCommand(task.Id, TaskItemStatus.InProgress);

        var result = await handler.Handle(command, default);

        result.Status.Should().Be(TaskItemStatus.InProgress);
        db.Tasks.Single(t => t.Id == task.Id).Status.Should().Be(TaskItemStatus.InProgress);
    }
}
