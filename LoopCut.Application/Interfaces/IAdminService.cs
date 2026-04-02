using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IAdminService
    {
        Task GetPaymentHistory(int pageIndex, int pageSize);
        Task GetLogUserActive(int pageIndex, int pageSize);
        

    }
}
