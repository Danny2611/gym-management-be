using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.Services;
using GymManagement.Infrastructure.Data;
using GymManagement.Infrastructure.Repositories;

namespace GymManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // MongoDB Context
            services.AddSingleton<MongoDbContext>();

            // Repositories
            services.AddScoped<IMemberRepository, MemberRepository>();

            // Services
            services.AddScoped<IMemberService, MemberService>();

            return services;
        }
    }
}