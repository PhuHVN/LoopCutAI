using LoopCut.Domain.Entities;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.DatabaseSettings;
using LoopCut.Infrastructure.Implemention;

namespace LoopCut.Infrastructure.Repository
{
    public class ServicePlanRepository : GenericRepository<ServicePlans>, IServicePlanRepository
    {
        public ServicePlanRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
