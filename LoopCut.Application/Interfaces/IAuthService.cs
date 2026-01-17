using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.DTOs.LoginDtos;
using LoopCut.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginEmail(LoginRequest login);
        Task<AuthResponse> LoginGoogle(LoginGoogleRequest login);
        Task<AccountResponse> Register(AccountRequest account);
        Task<AccountResponse> CurrentUser();
    }
}
