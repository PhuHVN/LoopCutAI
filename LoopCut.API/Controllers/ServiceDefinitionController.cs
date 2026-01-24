
using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    public class ServiceDefinitionController : ControllerBase
    {
        private readonly IServiceDefinitionManager _serviceDefinitionManager;
        public ServiceDefinitionController(IServiceDefinitionManager serviceDefinitionManager)
        {
            _serviceDefinitionManager = serviceDefinitionManager;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all services with pagination")]
        public async Task<IActionResult> GetAllServices([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null)
        {
            var result = await _serviceDefinitionManager.GetAllServices(pageIndex, pageSize, name);
            return Ok(ApiResponse<BasePaginatedList<ServiceResponse>>.OkResponse(result, "Get all services successful!", "200"));
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a service by ID")]
        public async Task<IActionResult> GetServiceById(string id)
        {
            var result = await _serviceDefinitionManager.GetServiceById(id);
            return Ok(ApiResponse<ServiceResponse>.OkResponse(result, "Get service by ID successful!", "200"));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new service")]
        public async Task<IActionResult> CreateService([FromBody] ServiceRequestV1 serviceRequest)
        {
            var result = await _serviceDefinitionManager.CreateService(serviceRequest);
            return Ok(ApiResponse<ServiceResponse>.OkResponse(result, "Service created successfully!", "201"));
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a service by ID")]
        public async Task<IActionResult> UpdateService([FromRoute] string id, [FromBody] ServiceRequestV1 serviceRequest)
        {
            var result = await _serviceDefinitionManager.UpdateService(id, serviceRequest);
            return Ok(ApiResponse<ServiceResponse>.OkResponse(result, "Service updated successfully!", "200"));
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a service by ID")]
        public async Task<IActionResult> DeleteService([FromRoute] string id)
        {
            await _serviceDefinitionManager.DeleteService(id);
            return Ok(ApiResponse<string>.OkResponse("Service deleted successfully", "200"));
        }

        // Add Service Plan to a Service
        [Authorize(Roles = "Admin")]
        [HttpPost("{serviceId}/plans")]
        [SwaggerOperation(Summary = "Add a service plan to a service")]
        public async Task<IActionResult> AddServicePlan([FromRoute] string serviceId, [FromBody] ServicePlanRequestV1 servicePlanRequest)
        {
            var result = await _serviceDefinitionManager.AddServicePlan(serviceId, servicePlanRequest);
            return Ok(ApiResponse<ServiceResponse>.OkResponse(result, "Service plan added successfully!", "201"));
        }
    }
}