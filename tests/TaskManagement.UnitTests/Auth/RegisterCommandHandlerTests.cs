using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Auth.Commands.Register;
using TaskManagement.Domain.Enums;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Auth;

public sealed class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmailExists_ThrowsConflictException()
    {
        var db = TestDbContextFactory.Create();
        var hasher = new FakePasswordHasher();
        var tokenGenerator = new FakeJwtTokenGenerator();
        var existing = new TaskManagement.Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = "exists@test.local",
            PasswordHash = hasher.Hash("Password1"),
            FullName = "Existing User",
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(existing);
        await db.SaveChangesAsync();

        var handler = new RegisterCommandHandler(db, hasher, tokenGenerator);
        var command = new RegisterCommand(existing.Email, "Password1", "New User");

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserAndReturnsAuthResponse()
    {
        var db = TestDbContextFactory.Create();
        var hasher = new FakePasswordHasher();
        var tokenGenerator = new FakeJwtTokenGenerator();
        var handler = new RegisterCommandHandler(db, hasher, tokenGenerator);

        var command = new RegisterCommand("new@test.local", "Password1", "New User");

        var result = await handler.Handle(command, default);

        result.Email.Should().Be(command.Email);
        result.FullName.Should().Be(command.FullName);
        result.Token.Should().Be("test-token");

        var savedUser = db.Users.Single(u => u.Email == command.Email);
        savedUser.PasswordHash.Should().Be(hasher.Hash(command.Password));
        savedUser.Role.Should().Be(UserRole.User);
    }
}
