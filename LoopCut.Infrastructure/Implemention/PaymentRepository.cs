using LoopCut.Application.Interfaces;
using LoopCut.Domain.Entities;
using LoopCut.Infrastructure.DatabaseSettings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Infrastructure.Implemention
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetPaymentForUpdateAsync(string orderCode)
        {
            return await _context.Payments
                .FromSqlInterpolated($@"
                    SELECT * FROM Payments WITH (UPDLOCK, ROWLOCK) 
                    WHERE OrderCode = {orderCode}")
                .Include(x => x.Membership)
                .FirstOrDefaultAsync();
        }
    }
}
