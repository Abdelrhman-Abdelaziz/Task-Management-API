using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
