using LoopCut.Domain.Entities;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.DatabaseSettings;
using LoopCut.Infrastructure.Implemention;

namespace LoopCut.Infrastructure.Repository
{
    public class AccountRepository : GenericRepository<Accounts>, IAccountRepository
    {
        public AccountRepository(AppDbContext appDb) : base(appDb)
        {
        }
    }
}
