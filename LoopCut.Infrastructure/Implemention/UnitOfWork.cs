using LoopCut.Domain.Abstractions;
using LoopCut.Domain.IRepository;
using LoopCut.Infrastructure.DatabaseSettings;
using Microsoft.EntityFrameworkCore.Storage;


namespace LoopCut.Infrastructure.Implemention
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private Dictionary<Type, object> repositories;

        public IAccountRepository AccountRepository { get; private set; }
        public IServicePlanRepository ServicePlanRepository { get; private set; }
        public IServiceRepository ServiceRepository { get; private set; }
        public ISubscriptionRepository SubscriptionRepository { get; private set; }

        public ISubscriptionEmailLogRepository SubscriptionEmailLogRepository { get; private set; }

        public UnitOfWork(
            AppDbContext context, 
            IAccountRepository accountRepository,
            IServicePlanRepository servicePlanRepository,
            IServiceRepository serviceRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionEmailLogRepository subscriptionEmailLogRepository)
        {
            _context = context;
            repositories = new Dictionary<Type, object>();
            AccountRepository = accountRepository;
            ServicePlanRepository = servicePlanRepository;
            ServiceRepository = serviceRepository;
            SubscriptionRepository = subscriptionRepository;
            SubscriptionEmailLogRepository = subscriptionEmailLogRepository;
        }
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
                _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollBackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<T>(_context);
                repositories.Add(type, repositoryInstance);
            }
            return (IGenericRepository<T>)repositories[type];
        }

        public bool HasActiveTransaction()
        {
            throw new NotImplementedException();
        }

        public async Task RollBackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
