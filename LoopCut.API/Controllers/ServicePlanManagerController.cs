
using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.DTOs.ServicePlanDTO;
using LoopCut.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/service-plans")]
    [ApiController]
    public class ServicePlanManagerController : ControllerBase
    {
        private readonly IServicePlanManager _servicePlanManager;

        public ServicePlanManagerController(IServicePlanManager servicePlanManager)
        {
            _servicePlanManager = servicePlanManager;
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get service plans by ID")]
        public async Task<IActionResult> GetServicePlanById(string id)
        {
            var servicePlan = await _servicePlanManager.GetServicePlansByIdAsync(id);
            return Ok(ApiResponse<ServicePlanResponse>.OkResponse(servicePlan, "Get service plan by ID successful!", "200"));
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update service plan by ID")]
        public async Task<IActionResult> UpdateServicePlanById([FromRoute] string id, [FromBody] ServicePlanRequestV1 servicePlanRequestV1)
        {
            var updatedServicePlan = await _servicePlanManager.UpdateServicePlanAsync(id, servicePlanRequestV1);
            return Ok(ApiResponse<ServicePlanResponse>.OkResponse(updatedServicePlan, "Service plan updated successfully!", "200"));
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete service plan by ID")]
        public async Task<IActionResult> DeleteServicePlanById(string id)
        {
            var deletedServicePlan = await _servicePlanManager.DeleteServicePlanAsync(id);
            return Ok(ApiResponse<ServicePlanResponse>.OkResponse(deletedServicePlan, "Service plan deleted successfully!", "200"));
        }
    }
}
