using CleanArchitecture.Application.Auth;
using CleanArchitecture.Application.Auth.DTOs;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Moq;
using Xunit;

namespace CleanArchitecture.Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _authService = new AuthService(_userRepoMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFalse_WhenUserExists()
        {
            _userRepoMock.Setup(r => r.GetByUsernameAsync("existing")).ReturnsAsync(new User());

            var result = await _authService.RegisterAsync(new RegisterRequest
            {
                Username = "existing",
                Password = "123"
            });

            Assert.False(result);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnTrue_WhenUserIsNew()
        {
            _userRepoMock.Setup(r => r.GetByUsernameAsync("new")).ReturnsAsync((User?)null);

            var result = await _authService.RegisterAsync(new RegisterRequest
            {
                Username = "new",
                Password = "123"
            });

            _userRepoMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenInvalidCredentials()
        {
            _userRepoMock.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

            var result = await _authService.LoginAsync(new LoginRequest
            {
                Username = "unknown",
                Password = "123"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResult_WhenValid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("123")))
            };

            _userRepoMock.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);
            _jwtProviderMock.Setup(p => p.GenerateToken(user)).Returns("token");

            var result = await _authService.LoginAsync(new LoginRequest
            {
                Username = "admin",
                Password = "123"
            });

            Assert.NotNull(result);
            Assert.Equal("token", result.AccessToken);
        }
    }
}
