using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Domain.Entities;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Projects;

public sealed class DeleteProjectCommandHandlerTests
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
        var handler = new DeleteProjectCommandHandler(db, currentUser);
        var command = new DeleteProjectCommand(project.Id);

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_RemovesProject()
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
        var handler = new DeleteProjectCommandHandler(db, currentUser);
        var command = new DeleteProjectCommand(project.Id);

        await handler.Handle(command, default);

        db.Projects.Should().BeEmpty();
    }
}
