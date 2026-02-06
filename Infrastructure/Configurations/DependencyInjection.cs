using Application.BackgroundServices;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using Application.Mapping;
using Application.Services;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// Provides extension methods for configuring infrastructure-related services and dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the infrastructure layer dependencies, including Database Context, Repositories, and Domain Services.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="configuration">The configuration instance to retrieve connection strings and settings.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 1. Cấu hình Database (PostgreSQL)
            services.AddDbContext<ChemXlabContext>(options =>
                options.UseNpgsql(connectionString)
            );

            // 2. Cấu hình Redis Cache (QUAN TRỌNG CHO OTP & CACHING)
            services.AddStackExchangeRedisCache(options =>
            {
                // Lấy chuỗi kết nối từ appsettings.json, mặc định là localhost:6379 nếu không tìm thấy
                options.Configuration = configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
                options.InstanceName = "ChemXLab_"; // Prefix để tránh trùng key với app khác trên cùng Redis server
            });

            // 3. Cấu hình AutoMapper & Generic Repository
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 4. Đăng ký UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 5. Đăng ký các Domain Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();

            // Đăng ký RedisService (Wrapper cho IDistributedCache)
            services.AddScoped<IRedisService, RedisService>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();

            // 6. Đăng ký Repositories cụ thể
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPackageService, PackageService>();

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionService, SubsctiptionService>();

            services.AddHostedService<PaymentBackrgroundService>();
            services.AddHostedService<SubscriptionBackgroundService>();

            return services;
        }
    }
}