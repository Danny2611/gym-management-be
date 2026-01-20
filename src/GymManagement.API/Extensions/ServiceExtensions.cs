
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Repositories.Public;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Interfaces.Services.Public;
using GymManagement.Application.Interfaces.Services.User;
using GymManagement.Application.Services.Admin;
using GymManagement.Application.Services.Public;
using GymManagement.Application.Services.User;
using GymManagement.Infrastructure.Data;
using GymManagement.Infrastructure.Identity;
using GymManagement.Infrastructure.Repositories.Admin;
using GymManagement.Infrastructure.Repositories.Public;
using GymManagement.Infrastructure.Repositories.User;
using GymManagement.Infrastructure.Services;

namespace GymManagement.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // MongoDB Context
            services.AddSingleton<MongoDbContext>();

            // Register HttpClient for MoMo
            services.AddHttpClient<IMoMoPaymentService, MoMoPaymentService>();



            // JWT Token Generator
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Email Service
            services.AddScoped<IEmailService, EmailService>();


            // Repositories

            //Member
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IPackageDetailRepository, PackageDetailRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<ITrainerRepository, TrainerRepository>();
            services.AddScoped<IWorkoutScheduleRepository, WorkoutScheduleRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IProgressRepository, ProgressRepository>();

            //Admin
            services.AddScoped<IAdminMemberRepository, AdminMemberRepository>();
            services.AddScoped<IAdminMembershipRepository, AdminMembershipRepository>();
            services.AddScoped<IAdminPackageRepository, AdminPackageRepository>();
            services.AddScoped<IAdminTrainerRepository, AdminTrainerRepository>();
            services.AddScoped<IAdminAppointmentRepository, AdminAppointmentRepository>();
            services.AddScoped<IAdminPromotionRepository, AdminPromotionRepository>();
            services.AddScoped<IAdminPaymentRepository, AdminPaymentRepository>();
            services.AddScoped<IAdminRevenueReportRepository, AdminRevenueReportRepository>();
            services.AddScoped<IAdminMemberReportRepository, AdminMemberReportRepository>();
            services.AddScoped<IAdminDashboardReportRepository, AdminDashboardReportRepository>();

            //public
            services.AddScoped<IBlogPostRepository, BlogPostRepository>();
            services.AddScoped<IBlogCategoryRepository, BlogCategoryRepository>();



            // Services

            // user
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITrainerService, TrainerService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IWorkoutScheduleService, WorkoutScheduleService>();
            services.AddScoped<IProgressService, ProgressService>();

            // admin
            services.AddScoped<IAdminMemberService, AdminMemberService>();
            services.AddScoped<IAdminMembershipService, AdminMembershipService>();
            services.AddScoped<IAdminPackageService, AdminPackageService>();
            services.AddScoped<IAdminTrainerService, AdminTrainerService>();
            services.AddScoped<IAdminAppointmentService, AdminAppointmentService>();
            services.AddScoped<IAdminPromotionService, AdminPromotionService>();
            services.AddScoped<IAdminPaymentService, AdminPaymentService>();
            services.AddScoped<IAdminRevenueReportService, AdminRevenueReportService>();
            services.AddScoped<IAdminMemberReportService, AdminMemberReportService>();
            services.AddScoped<IAdminDashboardReportService, AdminDashboardReportService>();

            //public
            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<IBlogCategoryService, BlogCategoryService>();

            // social
            services.AddScoped<ISocialAuthService, SocialAuthService>();

            return services;
        }
    }
}