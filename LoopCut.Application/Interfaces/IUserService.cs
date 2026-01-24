using LoopCut.Domain.Entities;

namespace LoopCut.Application.Interfaces
{
    public interface IUserService
    {
        Task<Accounts> GetCurrentUserLoginAsync();
    }
}
