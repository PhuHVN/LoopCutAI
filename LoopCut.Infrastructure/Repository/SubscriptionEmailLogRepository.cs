using LoopCut.Domain.Entities;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.DatabaseSettings;
using LoopCut.Infrastructure.Implemention;

namespace LoopCut.Infrastructure.Repository
{
    public class SubscriptionEmailLogRepository : GenericRepository<SubscriptionEmailLog>, ISubscriptionEmailLogRepository
    {
        public SubscriptionEmailLogRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
