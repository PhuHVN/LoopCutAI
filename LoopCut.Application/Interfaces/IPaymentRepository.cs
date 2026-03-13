using LoopCut.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentForUpdateAsync(string orderCode);
    }
}
