using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using TaskManagement.Domain.Entities;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Tasks;

public sealed class GetTasksByProjectQueryHandlerTests
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
        var handler = new GetTasksByProjectQueryHandler(db, currentUser);
        var query = new GetTasksByProjectQuery(project.Id);

        Func<Task> action = async () => await handler.Handle(query, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_ReturnsProjectTasks()
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
        var handler = new GetTasksByProjectQueryHandler(db, currentUser);
        var query = new GetTasksByProjectQuery(project.Id);

        var result = await handler.Handle(query, default);

        result.Should().ContainSingle(t => t.Id == task.Id);
    }
}
