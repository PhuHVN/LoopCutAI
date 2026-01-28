using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.DTOs.ServicePlanDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LoopCut.Application.Services
{
    public class ServicePlanManager : IServicePlanManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<ServicePlanManager> _logger;

        public ServicePlanManager(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<ServicePlanManager> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        public async Task<ServicePlanResponse> DeleteServicePlanAsync(string id)
        {
            var servicePlan = await _unitOfWork.ServicePlanRepository.GetByIdAsync(id);

            if (servicePlan == null || servicePlan.status == ServicePlanEnums.Inactive)
            {
                throw new KeyNotFoundException($"Service plan with ID {id} not found.");
            }

            servicePlan.status = ServicePlanEnums.Inactive;
            servicePlan.LastUpdatedAt = DateTime.UtcNow;
            var user = await _userService.GetCurrentUserLoginAsync();

            await _unitOfWork.ServicePlanRepository.UpdateAsync(servicePlan);

            return new ServicePlanResponse
            {
                Id = servicePlan.Id,
                PlanName = servicePlan.PlanName,
                Price = servicePlan.Price,
                BillingCycleEnums = servicePlan.BillingCycleEnums,
                Status = servicePlan.status,
                CreatedAt = servicePlan.CreatedAt,
                LastUpdatedAt = servicePlan.LastUpdatedAt,
                ModifiedByName = user.FullName
            };
        }

        public async Task<ServicePlanResponse> GetServicePlansByIdAsync(string id)
        {
            var user = await _userService.GetCurrentUserLoginAsync();

            if (user.Role == RoleEnum.Admin)
            {
                var adminServicePlan = await _unitOfWork.ServicePlanRepository.FindAsync(sp => sp.Id == id && sp.status == ServicePlanEnums.Active);
                if (adminServicePlan == null)
                {
                    throw new KeyNotFoundException($"Service plan not found.");
                }
                return new ServicePlanResponse
                {
                    Id = adminServicePlan.Id,
                    PlanName = adminServicePlan.PlanName,
                    Price = adminServicePlan.Price,
                    BillingCycleEnums = adminServicePlan.BillingCycleEnums,
                    Status = adminServicePlan.status,
                    CreatedAt = adminServicePlan.CreatedAt,
                    LastUpdatedAt = adminServicePlan.LastUpdatedAt,
                    ModifiedByName = adminServicePlan.ModifiedBy?.FullName
                };
            }

            var servicePlan = await _unitOfWork.ServicePlanRepository.FindAsync(sp => sp.Id == id && sp.status == ServicePlanEnums.Active && sp.ModifiedByID == user.Id);

            if (servicePlan == null)
            {
                throw new KeyNotFoundException($"Service plan not found.");
            }


            return new ServicePlanResponse
            {
                Id = servicePlan.Id,
                PlanName = servicePlan.PlanName,
                Price = servicePlan.Price,
                BillingCycleEnums = servicePlan.BillingCycleEnums,
                Status = servicePlan.status,
                CreatedAt = servicePlan.CreatedAt,
                LastUpdatedAt = servicePlan.LastUpdatedAt,
                ModifiedByName = servicePlan.ModifiedBy?.FullName
            };
        }

        public async Task<ServicePlanResponse> UpdateServicePlanAsync(string id, ServicePlanRequestV1 servicePlanRequestV1)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var servicePlan = await _unitOfWork.ServicePlanRepository.GetByIdAsync(id);

            // Check permissions
            if (servicePlan?.ModifiedByID != user.Id)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this service plan.");
            }

            if (servicePlan == null || servicePlan.status == ServicePlanEnums.Inactive)
            {
                throw new KeyNotFoundException($"Service plan with ID {id} not found.");
            }

            servicePlan.PlanName = servicePlanRequestV1.PlanName;
            servicePlan.Price = servicePlanRequestV1.Price;
            servicePlan.BillingCycleEnums = servicePlanRequestV1.BillingCycleEnums ?? BillingCycleEnums.None;
            servicePlan.LastUpdatedAt = DateTime.UtcNow;
            servicePlan.ModifiedBy = user;
            servicePlan.ModifiedByID = user.Id;

            try
            {
                await _unitOfWork.ServicePlanRepository.UpdateAsync(servicePlan);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service plan with ID {ServicePlanId}", id);
                throw;
            }

            return new ServicePlanResponse
            {
                Id = servicePlan.Id,
                PlanName = servicePlan.PlanName,
                Price = servicePlan.Price,
                BillingCycleEnums = servicePlan.BillingCycleEnums,
                Status = servicePlan.status,
                CreatedAt = servicePlan.CreatedAt,
                LastUpdatedAt = servicePlan.LastUpdatedAt,
                ModifiedByName = user.FullName
            };
        }
    }
}
