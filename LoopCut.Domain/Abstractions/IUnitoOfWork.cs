using LoopCut.Domain.IRepository;

namespace LoopCut.Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackAsync();
        bool HasActiveTransaction();


        // Interface Repository Pattern
        public IAccountRepository AccountRepository { get; }
        public IServicePlanRepository ServicePlanRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }

        public ISubscriptionEmailLogRepository SubscriptionEmailLogRepository { get;}
        }
}
