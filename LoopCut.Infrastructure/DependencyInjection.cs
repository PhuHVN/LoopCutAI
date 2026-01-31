
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.Implemention;
using LoopCut.Infrastructure.Repository;
using LoopCut.Infrastructure.Seeder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoopCut.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Infrastructure service registrations go here
            services.AddLogging();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<SeederData>();
            // DI for repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IServicePlanRepository, ServicePlanRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionEmailLogRepository, SubscriptionEmailLogRepository>();
        }

    }
}
