using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.Services;
using GymManagement.Infrastructure.Data;
using GymManagement.Infrastructure.Identity;
using GymManagement.Infrastructure.Repositories;

namespace GymManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // MongoDB Context
            services.AddSingleton<MongoDbContext>();

            // JWT Token Generator
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Repositories
            services.AddScoped<IMemberRepository, MemberRepository>();

            // Services
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}