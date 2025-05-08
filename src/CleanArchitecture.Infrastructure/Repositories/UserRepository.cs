using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddUserAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
