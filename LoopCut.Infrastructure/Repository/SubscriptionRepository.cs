using LoopCut.Domain.Entities;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.DatabaseSettings;
using LoopCut.Infrastructure.Implemention;

namespace LoopCut.Infrastructure.Repository
{
    public class SubscriptionRepository : GenericRepository<Subscriptions>, ISubscriptionRepository
    {
        public SubscriptionRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
