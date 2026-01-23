
using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.Interfaces;
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
            return Ok(result);
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a service by ID")]
        public async Task<IActionResult> GetServiceById(string id)
        {
            var result = await _serviceDefinitionManager.GetServiceById(id);
            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new service")]
        public async Task<IActionResult> CreateService([FromBody] ServiceRequestV1 serviceRequest)
        {
            var result = await _serviceDefinitionManager.CreateService(serviceRequest);
            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a service by ID")]
        public async Task<IActionResult> UpdateService([FromRoute] string id, [FromBody] ServiceRequestV1 serviceRequest)
        {
            var result = await _serviceDefinitionManager.UpdateService(id, serviceRequest);
            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a service by ID")]
        public async Task<IActionResult> DeleteService([FromRoute] string id)
        {
            await _serviceDefinitionManager.DeleteService(id);
            return Ok("Service deleted successfully");
        }
    }
}
