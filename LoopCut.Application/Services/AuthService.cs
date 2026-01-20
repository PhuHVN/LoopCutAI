using AutoMapper;
using Google.Apis.Auth;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.DTOs.LoginDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;
        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _http = http;
        }

        public async Task<string> GenerateJwtToken(Accounts accounts)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration!["Jwt:SecretKey"]);
                var account = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Id == accounts.Id && x.Status == StatusEnum.Active);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, accounts.Id.ToString()),
                        new Claim(ClaimTypes.Email, accounts.Email),
                        new Claim(ClaimTypes.Role, accounts.Role.ToString()),
                        new Claim("Status", accounts.Status.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, accounts.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);

            }
            catch (Exception ex)
            {
                throw new Exception("Error generating JWT token", ex);
            }
        }

        public async Task<AuthResponse> LoginEmail(LoginRequest request)
        {
            var existingAccount = await _unitOfWork.GetRepository<Accounts>()
                .FindAsync(a => a.Email == request.Email && a.Status == StatusEnum.Active);
            if (existingAccount == null)
            {
                throw new UnauthorizedAccessException("Account not found.");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, existingAccount.Password))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
            if (existingAccount.Status != Domain.Enums.StatusEnum.Active)
            {
                throw new UnauthorizedAccessException("Account is not active.");
            }
            return new AuthResponse
            {
                Token = await GenerateJwtToken(existingAccount),
                UserId = existingAccount.Id,
                Role = existingAccount.Role
            };
        }

        public async Task<AuthResponse> LoginGoogle(LoginGoogleRequest login)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(login.IdToken);
                var email = payload.Email;
                var fullName = payload.Name;
                var existingAccount = await _unitOfWork.GetRepository<Accounts>()
                    .FindAsync(a => a.Email == email && a.Status == StatusEnum.Active);
                if (existingAccount == null)
                {
                    throw new UnauthorizedAccessException("Account not found.");
                    //logic add new account 
                }
                return new AuthResponse
                {
                    Token = await GenerateJwtToken(existingAccount),
                    UserId = existingAccount.Id,
                    Role = existingAccount.Role
                };
            }catch(AuthenticationException e)
            {
                throw new UnauthorizedAccessException("Error during Google login", e);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during Google login", ex);
            }
        }

        public async Task<AccountResponse> CurrentUser()
        {
            var context = _http.HttpContext;
            var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }
            var user = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Id == userId && x.Status == StatusEnum.Active);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            return _mapper.Map<AccountResponse>(user);
        }

        public async Task<AccountResponse> Register(AccountRequest account)
        {
            var existingAccount = await  _unitOfWork.GetRepository<Accounts>()
                .FindAsync(a => a.Email == account.Email && a.Status == StatusEnum.Active);
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
            return _mapper.Map<AccountResponse>(newAccount);
        }
    }
}
