using TaskManagement.Application.Interfaces;

namespace TaskManagement.UnitTests.Common;

public sealed class FakeCurrentUserService : ICurrentUserService
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = "user@test.local";
    public string Role { get; set; } = "User";
}
