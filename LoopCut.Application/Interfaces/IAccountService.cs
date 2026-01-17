using LoopCut.Application.DTOs.AccountDtos;
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
        Task<AccountResponse> CreateAccount(AccountRequest account);
        Task<AccountResponse> UpdateAccount(AccountRequest account);
        Task<AccountResponse> GetAccountById(string id);
        Task<BasePaginatedList<AccountResponse>> GetAllAccounts();
        Task DeleteAccount(string id);
    }
}
