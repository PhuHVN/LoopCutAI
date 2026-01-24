using AutoMapper;
using FluentValidation;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Application.Validatior;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;

        }
        public async Task<AccountResponse> CreateAccount(AccountRequest account)
        {
            var existingAccount = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Email.ToLower() == account.Email.ToLower() && x.Status == StatusEnum.Active);
            if (existingAccount != null)
            {
                throw new ArgumentException("Account with the provided email already exists.");
            }
            var passwordHashing = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var newAccount = new Accounts
            {
                Email = account.Email,
                Password = passwordHashing,
                FullName = account.FullName,
                Address = account.Address,
                PhoneNumber = account.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                Role = RoleEnum.User,
                Status = StatusEnum.Active
            };
            await _unitOfWork.GetRepository<Accounts>().InsertAsync(newAccount);
            await _unitOfWork.SaveChangesAsync();
            return mapper.Map<AccountResponse>(newAccount);

        }

        public async Task DeleteAccount(string id)
        {
            var account = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Id == id && x.Status == StatusEnum.Active);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            account.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Accounts>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AccountResponse> GetAccountById(string id)
        {
            var account = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Id == id && x.Status == StatusEnum.Active);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            return mapper.Map<AccountResponse>(account);
        }

        public async Task<BasePaginatedList<AccountResponse>> GetAllAccounts(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Accounts>().Entity.Where(x => x.Status == StatusEnum.Active).OrderByDescending(x => x.CreatedAt);
            var rs = await _unitOfWork.GetRepository<Accounts>().GetPagging(query, pageIndex, pageSize);
            return mapper.Map<BasePaginatedList<AccountResponse>>(rs);
        }

        public async Task<AccountResponse> UpdateAccount(string id, AccountRequest account)
        {
            var existingAccount = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Id == id && x.Status == StatusEnum.Active);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            var isUpdate = false;
            if (!string.IsNullOrEmpty(account.FullName) && existingAccount.FullName != account.FullName)
            {
                existingAccount.FullName = account.FullName;
                isUpdate = true;
            }
            if (!string.IsNullOrEmpty(account.Address) && existingAccount.Address != account.Address)
            {
                existingAccount.Address = account.Address;
                isUpdate = true;
            }
            if (!string.IsNullOrEmpty(account.PhoneNumber) && existingAccount.PhoneNumber != account.PhoneNumber)
            {
                existingAccount.PhoneNumber = account.PhoneNumber;
                isUpdate = true;
            }
            if (isUpdate)
            {
                existingAccount.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.GetRepository<Accounts>().UpdateAsync(existingAccount);
                await _unitOfWork.SaveChangesAsync();
            }
            return mapper.Map<AccountResponse>(existingAccount);

        }
    }
}
