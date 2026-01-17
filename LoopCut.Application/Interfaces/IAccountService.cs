using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Accounts> CreateAccount();
        Task<Accounts> UpdateAccount(Accounts account);
        Task<Accounts> GetAccountById(string id);
        Task<BasePaginatedList<Accounts>> GetAllAccounts();
        Task DeleteAccount(string id);
    }
}
