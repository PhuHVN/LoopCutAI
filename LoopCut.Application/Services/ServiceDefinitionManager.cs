using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace LoopCut.Application.Services
{
    public class ServiceDefinitionManager : IServiceDefinitionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceDefinitionManager> _logger;
        private readonly IAccountService _accountService;

        public ServiceDefinitionManager(
            IUnitOfWork unitOfWork, 
            ILogger<ServiceDefinitionManager> logger, 
            IAccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _accountService = accountService;
        }

        public Task<ServiceResponse> CreateService(ServiceRequestV1 serviceRequest)
        {
            // 1. Map ServiceRequestV1 to Service entity

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
