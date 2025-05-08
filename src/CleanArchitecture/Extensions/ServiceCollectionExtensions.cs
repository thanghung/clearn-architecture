using CleanArchitecture.Application.Auth;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Auth;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("InMemoryDb"));

            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<AuthService>();

            return services;
        }
    }
}
