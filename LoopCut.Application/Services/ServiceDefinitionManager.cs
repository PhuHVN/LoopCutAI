using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.DTOs.ServicePlanDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoopCut.Application.Services
{
    public class ServiceDefinitionManager : IServiceDefinitionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceDefinitionManager> _logger;
        private readonly IUserService _userService;

        public ServiceDefinitionManager(
            IUnitOfWork unitOfWork,
            ILogger<ServiceDefinitionManager> logger,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }



        public async Task<ServiceResponse> CreateService(ServiceRequestV1 serviceRequest)
        {
            // Get user form context
            var user = await _userService.GetCurrentUserLoginAsync();

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Map ServiceRequestV1 to Service entity
                var service = new ServiceDefinitions
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = serviceRequest.Name,
                    Description = serviceRequest.Description,
                    LogoUrl = serviceRequest.LogoUrl,
                    CreatedAt = DateTime.UtcNow,
                    Status = ServiceEnums.Active,
                    ModifiedByID = user.Id,
                    ModifiedBy = user

                };
                await _unitOfWork.ServiceRepository.InsertAsync(service);

                // 4. Check if ServicePlans are provided in the request
                if (serviceRequest.ServicePlans != null && serviceRequest.ServicePlans.Any())
                {
                    // 5. Map and save each ServicePlan entity to database
                    foreach (var planRequest in serviceRequest.ServicePlans)
                    {
                        var servicePlan = new ServicePlans
                        {
                            PlanName = planRequest.PlanName,
                            Price = planRequest.Price,
                            BillingCycleEnums = planRequest.BillingCycleEnums ?? BillingCycleEnums.None,
                            CreatedAt = DateTime.UtcNow,
                            status = ServicePlanEnums.Active,
                            ServiceDefinitionId = service.Id,
                            ModifiedByID = user.Id,
                            ModifiedBy = user,
                            ServiceDefinition = service
                        };
                        await _unitOfWork.ServicePlanRepository.InsertAsync(servicePlan);
                    }
                }
                // 6. Commit transaction
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var serviceResponse = new ServiceResponse
                {
                    Id = service.Id,
                    Name = service.Name,
                    Description = service.Description,
                    LogoUrl = service.LogoUrl,
                    CreatedAt = service.CreatedAt,
                    Status = service.Status,
                    ModifiedByName = user.FullName
                };
                return serviceResponse;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                _logger.LogError(ex, "Error when creating service");
                throw;
            }
        }

        public async Task DeleteService(string id)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var existingService = await _unitOfWork.ServiceRepository.GetByIdAsync(id);

            // Check permission
            if (user.Id != existingService.ModifiedByID)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this service.");
            }

            if (existingService == null || existingService.Status == ServiceEnums.Inactive)
            {
                throw new ArgumentException("Service not found");
            }


            existingService.Status = ServiceEnums.Inactive;
            existingService.ModifiedByID = user.Id;
            existingService.ModifiedBy = user;
            existingService.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.ServiceRepository.UpdateAsync(existingService);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BasePaginatedList<ServiceResponse>> GetAllServices(int pageIndex, int pageSize, string? name)
        {
            var user = await _userService.GetCurrentUserLoginAsync();
            var query = _unitOfWork.ServiceRepository.Entity.Where(s => s.Status == ServiceEnums.Active);

            if (user.Role != RoleEnum.Admin)
            {
                query = query.Where(s => s.ModifiedByID == user.Id);
            }

            query = query.Include(s => s.ServicePlans).ThenInclude(sp => sp.ModifiedBy);
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            var paginatedServices = await _unitOfWork.ServiceRepository.GetPagging(query, pageIndex, pageSize);

            var serviceResponses = paginatedServices.Items.Select(service => MapToServiceResponse(service)).ToList();
            return new BasePaginatedList<ServiceResponse>
            {
                Items = serviceResponses,
                TotalItems = paginatedServices.TotalItems,
                PageIndex = paginatedServices.PageIndex,
                TotalPages = paginatedServices.TotalPages,
                PageSize = paginatedServices.PageSize
            };
        }

        public async Task<ServiceResponse> GetServiceById(string id)
        {
            var user = await _userService.GetCurrentUserLoginAsync();

            if (user.Role == RoleEnum.Admin)
            {
                var adminService = await _unitOfWork.ServiceRepository.FindAsync(s => s.Id == id && s.Status == ServiceEnums.Active,
                include: s => s.Include(x => x.ServicePlans).ThenInclude(sp => sp.ModifiedBy));
                if (adminService == null)
                {
                    throw new ArgumentException("Service not found");
                }
                return MapToServiceResponse(adminService);
            }

            var existingService = await _unitOfWork.ServiceRepository.FindAsync(s => s.Id == id && s.Status == ServiceEnums.Active && s.ModifiedByID == user.Id,
                include: s => s.Include(x => x.ServicePlans).ThenInclude(sp => sp.ModifiedBy));

            if (existingService == null)
            {
                throw new ArgumentException("Service not found");
            }
            return MapToServiceResponse(existingService);
        }


        public async Task<ServiceResponse> UpdateService(string id, ServiceUpdateRequest serviceRequest)
        {
            var user = await _userService.GetCurrentUserLoginAsync();

            var existingService = await _unitOfWork.ServiceRepository.GetByIdAsync(id);

            // Check permission
            if (user.Id != existingService.ModifiedByID)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this service.");
            }

            if (existingService == null || existingService.Status == ServiceEnums.Inactive)
            {
                throw new ArgumentException("Service not found");
            }

            existingService.Name = serviceRequest.Name;
            existingService.Description = serviceRequest.Description;
            existingService.LogoUrl = serviceRequest.LogoUrl;
            existingService.LastUpdatedAt = DateTime.UtcNow;
            existingService.ModifiedByID = user.Id;
            existingService.ModifiedBy = user;
            try
            {
                await _unitOfWork.ServiceRepository.UpdateAsync(existingService);
                await _unitOfWork.SaveChangesAsync();
                return MapToServiceResponse(existingService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service");
                throw;
            }
        }

        public async Task<ServiceResponse> AddServicePlan(string serviceId, ServicePlanRequestV1 servicePlanRequest)
        {
            // 1. Find existing service
            var existingService = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId)
                ?? throw new ArgumentException("Service not found");

            var user = await _userService.GetCurrentUserLoginAsync();

            // Check permission
            if (user.Id != existingService.ModifiedByID)
            {
                throw new UnauthorizedAccessException("You do not have permission to add service plan to this service.");
            }

            // 2. Create new ServicePlan entity
            var servicePlan = new ServicePlans
            {
                PlanName = servicePlanRequest.PlanName,
                Price = servicePlanRequest.Price,
                BillingCycleEnums = servicePlanRequest.BillingCycleEnums ?? BillingCycleEnums.None,
                CreatedAt = DateTime.UtcNow,
                status = ServicePlanEnums.Active,
                ServiceDefinitionId = existingService.Id,
                ModifiedByID = user.Id,
                ModifiedBy = user,
                ServiceDefinition = existingService
            };
            // 3. Save to database
            try
            {
                await _unitOfWork.ServicePlanRepository.InsertAsync(servicePlan);
                await _unitOfWork.SaveChangesAsync();
                return MapToServiceResponse(existingService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding service plan");
                throw;
            }
        }


        private ServiceResponse MapToServiceResponse(ServiceDefinitions service)
        {
            return new ServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                LogoUrl = service.LogoUrl,
                CreatedAt = service.CreatedAt,
                LastUpdatedAt = service.LastUpdatedAt,
                Status = service.Status,
                ModifiedByName = service.ModifiedBy?.FullName,
                ServicePlans = service.ServicePlans?.Where(sp => sp.status == ServicePlanEnums.Active).Select(sp => new ServicePlanResponse
                {
                    Id = sp.Id,
                    PlanName = sp.PlanName,
                    Price = sp.Price,
                    BillingCycleEnums = sp.BillingCycleEnums,
                    CreatedAt = sp.CreatedAt,
                    LastUpdatedAt = sp.LastUpdatedAt,
                    Status = sp.status,
                    ModifiedByName = sp.ModifiedBy?.FullName
                }).ToList()
            };
        }
    }
}
