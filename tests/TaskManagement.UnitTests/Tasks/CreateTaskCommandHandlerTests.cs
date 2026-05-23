using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Tasks;

public sealed class CreateTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenProjectNotOwned_ThrowsNotFoundException()
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

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = otherUserId };
        var handler = new CreateTaskCommandHandler(db, currentUser);
        var command = new CreateTaskCommand(project.Id, "Task", "Desc", null, TaskPriority.Medium);

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_CreatesTask()
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

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = ownerId };
        var handler = new CreateTaskCommandHandler(db, currentUser);
        var command = new CreateTaskCommand(project.Id, "Task", "Desc", null, TaskPriority.High);

        var result = await handler.Handle(command, default);

        result.Title.Should().Be("Task");
        result.Priority.Should().Be(TaskPriority.High);
        result.ProjectId.Should().Be(project.Id);

        db.Tasks.Should().ContainSingle(t => t.Id == result.Id);
    }
}
