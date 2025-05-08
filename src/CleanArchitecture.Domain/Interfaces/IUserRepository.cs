using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task SaveChangesAsync();
    }
}
