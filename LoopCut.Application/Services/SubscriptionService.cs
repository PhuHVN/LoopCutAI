using System.Xml.Linq;
using LoopCut.Application.DTOs.SubscriptionDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoopCut.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(IUnitOfWork unitOfWork, IUserService userService, ILogger<SubscriptionService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        public async Task<SubscriptionResponseV1> CreateSubscriptionByUserAsync(SubscriptionRequest subscriptionRequest)
        {
            var user = await _userService.GetCurrentUserLoginAsync();

            // If a service plan id was provided, validate it exists. ServicePlanId can be null.
            if (!string.IsNullOrWhiteSpace(subscriptionRequest.ServicePlanId))
            {
                var servicePlan = await _unitOfWork.ServicePlanRepository.GetByIdAsync(subscriptionRequest.ServicePlanId!);
                if (servicePlan == null)
                {
                    throw new KeyNotFoundException("Service plan not found.");
                }
            }

            var subscription = new Subscriptions
            {
                AccountId = user.Id,
                Account = user,
                ServicePlanId = string.IsNullOrWhiteSpace(subscriptionRequest.ServicePlanId) ? null : subscriptionRequest.ServicePlanId,
                SubscriptionsName = subscriptionRequest.SubscriptionsName,
                StartDate = subscriptionRequest.StartDate,
                EndDate = subscriptionRequest.EndDate,
                Price = subscriptionRequest.Price,
                RemiderDays = subscriptionRequest.RemiderDays,
                CreatedAt = DateTime.UtcNow,
                Status = SubscriptionEnums.Active
            };

            try
            {
                await _unitOfWork.SubscriptionRepository.InsertAsync(subscription);
                await _unitOfWork.SaveChangesAsync();
                return MapToSubV1(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subscription for user {UserId}", user.Id);
                throw;

            }
        }

        public async Task<SubscriptionResponseV1> DeleteSubscriptionByUserAsync(string id)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            // Find the subscription by id
            var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
            if (subscription == null)
            {
                throw new KeyNotFoundException("Subscription not found.");
            }

            if (subscription.Status == SubscriptionEnums.Inactive)
            {
                throw new InvalidOperationException("Subscription is already deleted.");
            }
            // Check if the subscription belongs to the user
            if (subscription.AccountId != user.Id)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this subscription.");
            }

            // Soft delete by setting status to Inactive
            subscription.Status = SubscriptionEnums.Inactive;
            subscription.LastUpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.SubscriptionRepository.UpdateAsync(subscription);
                await _unitOfWork.SaveChangesAsync();
                return MapToSubV1(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting subscription {SubscriptionId} for user {UserId}", id, user.Id);
                throw;
            }

        }

        public async Task<BasePaginatedList<SubscriptionResponseV1>> GetAllSubscriptionsByUserLoginAsync(
            int pageIndex,
            int pageSize,
            string name,
            int reminderDays)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            // Don't include ServicePlan join if it could be null - handle null check in mapping
            var query = _unitOfWork.SubscriptionRepository.Entity.Where(s => s.Status != SubscriptionEnums.Inactive && s.AccountId == user.Id);
            query = query.Include(s => s.ServicePlan)
                         .ThenInclude(sp => sp.ServiceDefinition);


            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.SubscriptionsName.Contains(name));
            }

            if (reminderDays > 0)
            {
                query = query.Where(s => s.RemiderDays == reminderDays);
            }
            var paginatedSubscriptions = await _unitOfWork.SubscriptionRepository.GetPagging(
                query,
                pageIndex,
                pageSize
            );

            var subscriptionResponses = paginatedSubscriptions.Items
                .Select(s => MapToSubV1(s))
                .ToList();

            return new BasePaginatedList<SubscriptionResponseV1>
            {
                Items = subscriptionResponses,
                TotalItems = paginatedSubscriptions.TotalItems,
                PageIndex = paginatedSubscriptions.PageIndex,
                TotalPages = paginatedSubscriptions.TotalPages,
                PageSize = paginatedSubscriptions.PageSize
            };
        }

        public async Task<BasePaginatedList<SubscriptionResponseV2>> GetAllSubscriptionsByUserLoginAsync(int pageIndex, int pageSize)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var query = _unitOfWork.SubscriptionRepository.Entity
               .Where(s => s.Status != SubscriptionEnums.Inactive && s.AccountId == user.Id);

            // Include ServicePlan and ServiceDefinition - we'll check null in mapping
            query = query.Include(s => s.ServicePlan)
                 .ThenInclude(sp => sp.ServiceDefinition);
         
            var paginatedSubscriptions = await _unitOfWork.SubscriptionRepository.GetPagging(query, pageIndex, pageSize);

            var subscriptionResponses = paginatedSubscriptions.Items
                .Select(s => MapToSubV2(s))
                .ToList();

            return new BasePaginatedList<SubscriptionResponseV2>
            {
                Items = subscriptionResponses,
                TotalItems = paginatedSubscriptions.TotalItems,
                PageIndex = paginatedSubscriptions.PageIndex,
                TotalPages = paginatedSubscriptions.TotalPages,
                PageSize = paginatedSubscriptions.PageSize
            };
        }

        public async Task<BasePaginatedList<SubscriptionResponseV2>> GetAllSubscriptionsWithPlanAndServiceByManagerAsync(
            int pageIndex,
            int pageSize,
            string subName,
            int reminderDays,
            string serviceName)
        {
            var query = _unitOfWork.SubscriptionRepository.Entity
                .Where(s => s.Status != SubscriptionEnums.Inactive);

            // Include ServicePlan and ServiceDefinition - we'll check null in mapping
            query = query.Include(s => s.ServicePlan)
                 .ThenInclude(sp => sp.ServiceDefinition);

            if (!string.IsNullOrEmpty(subName))
            {
                query = query.Where(s => s.SubscriptionsName.Contains(subName));
            }
            if (reminderDays > 0)
            {
                query = query.Where(s => s.RemiderDays == reminderDays);
            }

            if (!string.IsNullOrEmpty(serviceName))
            {
                // Only filter by service name if ServicePlan exists and has ServiceDefinition
                query = query.Where(s => s.ServicePlan != null && s.ServicePlan.ServiceDefinition != null && s.ServicePlan.ServiceDefinition.Name.Contains(serviceName));
            }

            var paginatedSubscriptions = await _unitOfWork.SubscriptionRepository.GetPagging(query, pageIndex, pageSize);

            var subscriptionResponses = paginatedSubscriptions.Items
                .Select(s => MapToSubV2(s))
                .ToList();

            return new BasePaginatedList<SubscriptionResponseV2>
            {
                Items = subscriptionResponses,
                TotalItems = paginatedSubscriptions.TotalItems,
                PageIndex = paginatedSubscriptions.PageIndex,
                TotalPages = paginatedSubscriptions.TotalPages,
                PageSize = paginatedSubscriptions.PageSize
            };

        }

        public async Task<SubscriptionResponseV1> GetSubscriptionByIdByUserAsync(string id)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var subscription = await _unitOfWork.SubscriptionRepository.Entity
            .Include(s => s.ServicePlan)
           .ThenInclude(sp => sp.ServiceDefinition)
          .FirstOrDefaultAsync(s => s.Id == id && s.Status != SubscriptionEnums.Inactive && s.AccountId == user.Id);
            if (subscription == null)
            {
                throw new KeyNotFoundException("Subscription not found.");
            }
            return MapToSubV1(subscription);
        }

        public async Task<BasePaginatedList<SubscriptionResponseV2>> GetSubscriptionStatusByUserLoginAsync(int pageIndex, int pageSize)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var query = _unitOfWork.SubscriptionRepository.Entity
               .Where(s => s.Status != SubscriptionEnums.Inactive && s.AccountId == user.Id && s.EndDate <= DateTime.UtcNow.AddDays(3));

            // Include ServicePlan and ServiceDefinition - we'll check null in mapping
            query = query.Include(s => s.ServicePlan)
                 .ThenInclude(sp => sp.ServiceDefinition);

            var paginatedSubscriptions = await _unitOfWork.SubscriptionRepository.GetPagging(query, pageIndex, pageSize);

            var subscriptionResponses = paginatedSubscriptions.Items
                .Select(s => MapToSubV2(s))
                .ToList();

            return new BasePaginatedList<SubscriptionResponseV2>
            {
                Items = subscriptionResponses,
                TotalItems = paginatedSubscriptions.TotalItems,
                PageIndex = paginatedSubscriptions.PageIndex,
                TotalPages = paginatedSubscriptions.TotalPages,
                PageSize = paginatedSubscriptions.PageSize
            };
        }

        public async Task<SubscriptionResponseV2> GetSubscriptionWithPlanAndServiceByIdByManagerAsync(string id)
        {
            var subscription = await _unitOfWork.SubscriptionRepository.Entity
                .Include(s => s.ServicePlan).ThenInclude(sp => sp.ServiceDefinition)
                .FirstOrDefaultAsync(s => s.Id == id && s.Status != SubscriptionEnums.Inactive);

            if (subscription == null)
            {
                throw new KeyNotFoundException("Subscription not found.");
            }
            return MapToSubV2(subscription);
        }

        public async Task<SubscriptionResponseV1> UpdateSubscriptionByUserLoginAsync(string id, SubscriptionRequest subscriptionRequest)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(id);
            if (subscription == null || subscription.Status == SubscriptionEnums.Inactive)
            {
                throw new KeyNotFoundException("Subscription not found.");
            }
            if (subscription.AccountId != user.Id)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this subscription.");
            }

            // If a service plan id is provided, validate it exists; if null/empty, unlink the plan
            if (!string.IsNullOrWhiteSpace(subscriptionRequest.ServicePlanId))
            {
                var servicePlan = await _unitOfWork.ServicePlanRepository.GetByIdAsync(subscriptionRequest.ServicePlanId!);
                if (servicePlan == null)
                {
                    throw new KeyNotFoundException("Service plan not found.");
                }

                subscription.ServicePlanId = subscriptionRequest.ServicePlanId;
            }
            else
            {
                // allow setting ServicePlanId to null (unlink plan)
                subscription.ServicePlanId = null;
            }

            subscription.SubscriptionsName = subscriptionRequest.SubscriptionsName;
            subscription.StartDate = subscriptionRequest.StartDate;
            subscription.EndDate = subscriptionRequest.EndDate;
            subscription.Price = subscriptionRequest.Price;
            subscription.RemiderDays = subscriptionRequest.RemiderDays;
            subscription.LastUpdatedAt = DateTime.UtcNow;
            try
            {
                await _unitOfWork.SubscriptionRepository.UpdateAsync(subscription);
                await _unitOfWork.SaveChangesAsync();
                return MapToSubV1(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating subscription {SubscriptionId} for user {UserId}", id, user.Id);
                throw;
            }

        }

        private SubscriptionResponseV1 MapToSubV1(Subscriptions subscription)
        {
            return new SubscriptionResponseV1
            {
                Id = subscription.Id,
                ServicePlanId = subscription.ServicePlanId,
                SubscriptionsName = subscription.SubscriptionsName,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Price = subscription.Price,
                RemiderDays = subscription.RemiderDays,
                Status = subscription.Status,
                PlanId = subscription.ServicePlan?.Id ?? string.Empty,
                PlanName = subscription.ServicePlan?.PlanName ?? string.Empty,
                PlanPrice = subscription.ServicePlan?.Price ?? 0,
                BillingCycleEnums = subscription.ServicePlan?.BillingCycleEnums ?? BillingCycleEnums.None,
                ServiceId = subscription.ServicePlan?.ServiceDefinitionId ?? string.Empty,
                ServiceName = subscription.ServicePlan?.ServiceDefinition?.Name ?? string.Empty
            };
        }

        private SubscriptionResponseV2 MapToSubV2(Subscriptions subscription)
        {
            return new SubscriptionResponseV2
            {
                Id = subscription.Id,
                ServicePlanId = subscription.ServicePlanId,
                SubscriptionsName = subscription.SubscriptionsName,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Price = subscription.Price,
                RemiderDays = subscription.RemiderDays,
                CreatedAt = subscription.CreatedAt,
                LastUpdatedAt = subscription.LastUpdatedAt,
                Status = subscription.Status,
                PlanId = subscription.ServicePlan?.Id,
                PlanName = subscription.ServicePlan?.PlanName ?? string.Empty,
                PlanPrice = subscription.ServicePlan?.Price ?? 0,
                BillingCycleEnums = subscription.ServicePlan?.BillingCycleEnums ?? BillingCycleEnums.None,
                ServiceId = subscription.ServicePlan?.ServiceDefinitionId ?? string.Empty,
                ServiceName = subscription.ServicePlan?.ServiceDefinition?.Name ?? string.Empty
            };
        }
    }
}
