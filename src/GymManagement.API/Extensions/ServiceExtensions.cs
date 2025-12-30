using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.Services;
using GymManagement.Infrastructure.Data;
using GymManagement.Infrastructure.Identity;
using GymManagement.Infrastructure.Repositories;
using GymManagement.Infrastructure.Services;

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

            // Email Service
            services.AddScoped<IEmailService, EmailService>();

            // Repositories
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IPackageDetailRepository, PackageDetailRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Services
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            return services;
        }
    }
}