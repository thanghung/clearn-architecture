using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
