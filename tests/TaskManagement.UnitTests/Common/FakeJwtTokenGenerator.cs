using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.UnitTests.Common;

public sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(User user) => "test-token";
}
