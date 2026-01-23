using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LoopCut.Application.Services
{
    public class ServiceDefinitionManager : IServiceDefinitionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceDefinitionManager> _logger;
        private readonly IAuthService _accountService;

        public ServiceDefinitionManager(
            IUnitOfWork unitOfWork, 
            ILogger<ServiceDefinitionManager> logger, 
            IAuthService accountService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _accountService = accountService;
        }

        public async Task<ServiceResponse> CreateService(ServiceRequestV1 serviceRequest)
        {
            // Get user form context
            var user = await _accountService.CurrentUser();
            var existingUser = await _unitOfWork.AccountRepository.GetByIdAsync(user.Id) 
                ?? throw new ArgumentException("User not found");

            // 1. Map ServiceRequestV1 to Service entity
            var service = new ServiceDefinitions
            {
                Name = serviceRequest.Name,
                Description = serviceRequest.Description,
                LogoUrl = serviceRequest.LogoUrl,
                CreatedAt = DateTime.UtcNow,
                Status = ServiceEnums.Active,
                ModifiedByID = existingUser.Id,
                ModifiedBy = existingUser

            };

            // 2. Start transaction

            // 3. Save Service entity to database

            // 4. Check if ServicePlans are provided in the request

            // 5. Map and save each ServicePlan entity to database

            // 6. Commit transaction
            throw new NotImplementedException();
        }

        public Task DeleteService(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> GetServiceById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> UpdateService(string id, ServiceRequestV1 serviceRequest)
        {
            throw new NotImplementedException();
        }
    }
}
