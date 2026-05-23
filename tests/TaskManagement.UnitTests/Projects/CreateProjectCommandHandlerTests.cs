using FluentAssertions;

using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Domain.Enums;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Projects;

public sealed class CreateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_SetsOwnerAndReturnsDto()
    {
        var db = TestDbContextFactory.Create();
        var currentUser = new FakeCurrentUserService
        {
            UserId = Guid.NewGuid(),
            Email = "owner@test.local",
            Role = UserRole.User.ToString()
        };
        var handler = new CreateProjectCommandHandler(db, currentUser);

        var command = new CreateProjectCommand("Project A", "Desc");

        var result = await handler.Handle(command, default);

        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);

        var saved = db.Projects.Single(p => p.Id == result.Id);
        saved.UserId.Should().Be(currentUser.UserId);
    }
}
