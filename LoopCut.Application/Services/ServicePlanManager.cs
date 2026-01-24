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
            servicePlan.ModifiedBy = user;
            servicePlan.ModifiedByID = user.Id;

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
            var servicePlan = await _unitOfWork.ServicePlanRepository.GetByIdAsync(id);
            if (servicePlan == null || servicePlan.status == ServicePlanEnums.Inactive)
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
           var servicePlan = await _unitOfWork.ServicePlanRepository.GetByIdAsync(id);
            if (servicePlan == null || servicePlan.status == ServicePlanEnums.Inactive)
            {
                throw new KeyNotFoundException($"Service plan with ID {id} not found.");
            }

            servicePlan.PlanName = servicePlanRequestV1.PlanName;
            servicePlan.Price = servicePlanRequestV1.Price;
            servicePlan.BillingCycleEnums = servicePlanRequestV1.BillingCycleEnums;
            servicePlan.LastUpdatedAt = DateTime.UtcNow;

            var user = await _userService.GetCurrentUserLoginAsync();
            servicePlan.ModifiedBy = user;
            servicePlan.ModifiedByID = user.Id;

            try 
            {
                await _unitOfWork.ServicePlanRepository.UpdateAsync(servicePlan);
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
