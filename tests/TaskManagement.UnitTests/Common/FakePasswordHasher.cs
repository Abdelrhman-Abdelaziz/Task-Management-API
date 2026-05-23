using TaskManagement.Application.Interfaces;

namespace TaskManagement.UnitTests.Common;

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string hash)
        => hash == Hash(password);
}
