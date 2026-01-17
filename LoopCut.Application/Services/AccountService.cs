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
            var existingAccount = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Email.ToLower() == account.Email.ToLower());
            if(existingAccount != null)
            {
                throw new ArgumentException("Account with the provided email already exists.");
            }
            var passwordHashing = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var newAccount = new Accounts
            {
                Email = account.Email,
                Password =  passwordHashing,
                FullName = account.FullName,
                Address = account.Address,
                PhoneNumber = account.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                Role = RoleEnum.Customer,
                Status = StatusEnum.Active
            };
            await _unitOfWork.GetRepository<Accounts>().InsertAsync(newAccount);
            await _unitOfWork.SaveChangeAsync();
            return mapper.Map<AccountResponse>(newAccount);

        }

        public async Task DeleteAccount(string id)
        {
            var account = await _unitOfWork.GetRepository<Accounts>().GetByIdAsync(id);
            if(account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            await _unitOfWork.GetRepository<Accounts>().DeleteAsync(account);
        }

        public async Task<AccountResponse> GetAccountById(string id)
        {
            var account = await _unitOfWork.GetRepository<Accounts>().GetByIdAsync(id);
            if(account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            return mapper.Map<AccountResponse>(account);
        }

        public Task<BasePaginatedList<AccountResponse>> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public Task<AccountResponse> UpdateAccount(AccountRequest account)
        {
            throw new NotImplementedException();
        }
    }
}
