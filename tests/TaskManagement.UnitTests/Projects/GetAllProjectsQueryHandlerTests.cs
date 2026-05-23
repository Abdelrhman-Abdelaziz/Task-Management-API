using FluentAssertions;

using TaskManagement.Application.Features.Projects.Queries.GetAllProjects;
using TaskManagement.Domain.Entities;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Projects;

public sealed class GetAllProjectsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenMixedOwnership_ReturnsOnlyCurrentUserProjects()
    {
        var db = TestDbContextFactory.Create();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var owned = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Owned",
            Description = "Desc",
            UserId = ownerId,
            CreatedAt = DateTime.UtcNow
        };
        var other = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Other",
            Description = "Desc",
            UserId = otherUserId,
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.AddRange(owned, other);
        await db.SaveChangesAsync();

        var currentUser = new FakeCurrentUserService { UserId = ownerId };
        var handler = new GetAllProjectsQueryHandler(db, currentUser);

        var result = await handler.Handle(new GetAllProjectsQuery(), default);

        result.Should().HaveCount(1);
        result.Single().Id.Should().Be(owned.Id);
    }
}
