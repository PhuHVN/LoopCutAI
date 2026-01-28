using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.SubscriptionDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/subscriptions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all subscriptions with plan and service by manager with pagination")]
        public async Task<IActionResult> GetAllSubscriptionsWithPlanAndService(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? subName = null,
            [FromQuery] int reminderDays = 0,
            [FromQuery] string? serviceName = null)
        {
            var result = await _subscriptionService.GetAllSubscriptionsWithPlanAndServiceByManagerAsync(pageIndex, pageSize, subName, reminderDays, serviceName);
            return Ok(ApiResponse<BasePaginatedList<SubscriptionResponseV2>>.OkResponse(result, "Get all subscriptions successful!", "200"));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a subscription with plan and service by ID by manager")]
        public async Task<IActionResult> GetSubscriptionWithPlanAndServiceById([FromRoute] string id)
        {
            var result = await _subscriptionService.GetSubscriptionWithPlanAndServiceByIdByManagerAsync(id);
            return Ok(ApiResponse<SubscriptionResponseV2>.OkResponse(result, "Get subscription by ID successful!", "200"));
        }
    }
}
