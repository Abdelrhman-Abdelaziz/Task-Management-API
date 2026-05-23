using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;
using TaskManagement.Domain.Entities;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Projects;

public sealed class GetProjectByIdQueryHandlerTests
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
        var handler = new GetProjectByIdQueryHandler(db, currentUser);
        var query = new GetProjectByIdQuery(project.Id);

        Func<Task> action = async () => await handler.Handle(query, default);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_ReturnsProject()
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
        var handler = new GetProjectByIdQueryHandler(db, currentUser);
        var query = new GetProjectByIdQuery(project.Id);

        var result = await handler.Handle(query, default);

        result.Id.Should().Be(project.Id);
        result.Name.Should().Be(project.Name);
    }
}
