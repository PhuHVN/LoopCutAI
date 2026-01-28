using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLink(decimal amount, string currency, string description);

    }
}
