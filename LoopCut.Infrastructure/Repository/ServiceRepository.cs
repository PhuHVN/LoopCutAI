using LoopCut.Domain.Entities;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.DatabaseSettings;
using LoopCut.Infrastructure.Implemention;

namespace LoopCut.Infrastructure.Repository
{
    public class ServiceRepository : GenericRepository<Services>, IServiceRepository
    {
        public ServiceRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
