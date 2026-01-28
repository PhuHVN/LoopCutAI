using LoopCut.Application.DTOs.SubscriptionDTO;
using LoopCut.Domain.Abstractions;

namespace LoopCut.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseV1> CreateSubscriptionByUserAsync(SubscriptionRequest subscriptionRequest);

        Task<SubscriptionResponseV1> GetSubscriptionByIdByUserAsync(string id);

        Task<SubscriptionResponseV1> UpdateSubscriptionByUserLoginAsync(string id, SubscriptionRequest subscriptionRequest);

        Task<SubscriptionResponseV1> DeleteSubscriptionByUserAsync(string id);

        Task<BasePaginatedList<SubscriptionResponseV1>> GetAllSubscriptionsByUserLoginAsync(int pageIndex, int pageSize, string name, int reminderDays);


        Task<BasePaginatedList<SubscriptionResponseV2>> GetAllSubscriptionsWithPlanAndServiceByManagerAsync(int pageIndex, int pageSize, string subName, int reminderDays, string serviceName);

        Task<SubscriptionResponseV2> GetSubscriptionWithPlanAndServiceByIdByManagerAsync(string id);
    }
}
