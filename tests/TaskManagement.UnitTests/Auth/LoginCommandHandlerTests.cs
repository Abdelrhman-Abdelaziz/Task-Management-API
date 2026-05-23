using FluentAssertions;

using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Auth.Commands.Login;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Auth;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsUnauthorizedException()
    {
        var db = TestDbContextFactory.Create();
        var hasher = new FakePasswordHasher();
        var tokenGenerator = new FakeJwtTokenGenerator();
        var handler = new LoginCommandHandler(db, hasher, tokenGenerator);

        var command = new LoginCommand("missing@test.local", "Password1");

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_WhenPasswordInvalid_ThrowsUnauthorizedException()
    {
        var db = TestDbContextFactory.Create();
        var hasher = new FakePasswordHasher();
        var tokenGenerator = new FakeJwtTokenGenerator();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.local",
            PasswordHash = hasher.Hash("Password1"),
            FullName = "Test User",
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new LoginCommandHandler(db, hasher, tokenGenerator);
        var command = new LoginCommand(user.Email, "WrongPass1");

        Func<Task> action = async () => await handler.Handle(command, default);

        await action.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ReturnsAuthResponse()
    {
        var db = TestDbContextFactory.Create();
        var hasher = new FakePasswordHasher();
        var tokenGenerator = new FakeJwtTokenGenerator();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.local",
            PasswordHash = hasher.Hash("Password1"),
            FullName = "Test User",
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new LoginCommandHandler(db, hasher, tokenGenerator);
        var command = new LoginCommand(user.Email, "Password1");

        var result = await handler.Handle(command, default);

        result.Email.Should().Be(user.Email);
        result.FullName.Should().Be(user.FullName);
        result.Token.Should().Be("test-token");
    }
}
