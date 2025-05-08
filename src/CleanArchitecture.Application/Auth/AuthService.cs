using CleanArchitecture.Application.Auth.DTOs;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Application.Auth
{
    public class AuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtProvider _jwt;

        public AuthService(IUserRepository repo, IJwtProvider jwt)
        {
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _repo.GetByUsernameAsync(request.Username);
            if (user == null || user.PasswordHash != Hash(request.Password)) return null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _repo.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = _jwt.GenerateToken(user)
            };
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
        {
            var user = await _repo.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow) return null;

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5);

            await _repo.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = _jwt.GenerateToken(user),
                RefreshToken = user.RefreshToken
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var existing = await _repo.GetByUsernameAsync(request.Username);
            if (existing != null) return false;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = Hash(request.Password),
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            await _repo.AddUserAsync(user);
            await _repo.SaveChangesAsync();
            return true;
        }

        private string Hash(string password) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        private string GenerateRefreshToken() => Guid.NewGuid().ToString("N");
    }
}
