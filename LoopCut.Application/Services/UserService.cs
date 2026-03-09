using System.Security.Claims;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace LoopCut.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IHttpContextAccessor http, IUnitOfWork unitOfWork)
        {
            _http = http;
            _unitOfWork = unitOfWork;
        }

        public async Task<Accounts> GetCurrentUserLoginAsync()
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

            return user;
        }

        public string? GetIpAddressAsync()
        {
            var httpContext = _http.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("No HTTP context available.");
            }
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if(!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',').FirstOrDefault()?.Trim();
            }
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if(!string.IsNullOrEmpty(realIp))
            {
                return realIp.Trim();
            }
            return httpContext.Connection.RemoteIpAddress?.ToString();

        }
    }
}
