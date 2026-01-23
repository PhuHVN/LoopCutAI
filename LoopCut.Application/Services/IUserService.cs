using LoopCut.Domain.Entities;

namespace LoopCut.Application.Services
{
    public interface IUserService
    {
        Task<Accounts> GetCurrentUserLoginAsync();
    }
}
