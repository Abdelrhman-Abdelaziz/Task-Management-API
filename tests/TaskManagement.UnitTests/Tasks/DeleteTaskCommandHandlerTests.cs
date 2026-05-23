using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Domain.Entities;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Tasks;

public sealed class DeleteTaskCommandHandlerTests
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
        var handler = new DeleteTaskCommandHandler(db, currentUser);
        var command = new DeleteTaskCommand(task.Id);

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_RemovesTask()
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
        var handler = new DeleteTaskCommandHandler(db, currentUser);
        var command = new DeleteTaskCommand(task.Id);

        await handler.Handle(command, default);

        db.Tasks.Should().BeEmpty();
    }
}
