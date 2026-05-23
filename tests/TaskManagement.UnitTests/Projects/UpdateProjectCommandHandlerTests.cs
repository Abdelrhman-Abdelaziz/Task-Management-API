using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Domain.Entities;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Projects;

public sealed class UpdateProjectCommandHandlerTests
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
            Name = "Original",
            Description = "Desc",
            UserId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = otherUserId };
        var handler = new UpdateProjectCommandHandler(db, currentUser);
        var command = new UpdateProjectCommand(project.Id, "Updated", "New Desc");

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_UpdatesProject()
    {
        var db = TestDbContextFactory.Create();
        var ownerId = Guid.NewGuid();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            Description = "Desc",
            UserId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = ownerId };
        var handler = new UpdateProjectCommandHandler(db, currentUser);
        var command = new UpdateProjectCommand(project.Id, "Updated", "New Desc");

        var result = await handler.Handle(command, default);

        result.Name.Should().Be("Updated");
        result.Description.Should().Be("New Desc");

        var saved = db.Projects.Single(p => p.Id == project.Id);
        saved.Name.Should().Be("Updated");
        saved.Description.Should().Be("New Desc");
    }
}
