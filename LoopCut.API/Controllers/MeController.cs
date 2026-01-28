using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.SubscriptionDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/me")]
    [Authorize(Roles ="User")]
    [ApiController]
    public class MeController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public MeController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("subscriptions")]
        [SwaggerOperation(Summary = "Get all subscriptions by user login with pagination")]
        public async Task<IActionResult> GetAllSubcriptionByUserLogin(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? name = null, 
            [FromQuery] int reminderDays = 0)
        {
            var result = await _subscriptionService.GetAllSubscriptionsByUserLoginAsync(pageIndex, pageSize, name, reminderDays);
            return Ok(ApiResponse<BasePaginatedList<SubscriptionResponseV1>>.OkResponse(result, "Get all subscriptions successful!", "200"));
        }


        [HttpGet("subscriptions/{id}")]
        [SwaggerOperation(Summary = "Get a subscription by ID by user login")]
        public async Task<IActionResult> GetAllSubcriptionByIdByUserLogin([FromRoute] string id)
        {
            var result = await _subscriptionService.GetSubscriptionByIdByUserAsync(id);
            return Ok(ApiResponse<SubscriptionResponseV1>.OkResponse(result, "Get subscription by ID successful!", "200"));
        }

        [HttpPost("subscriptions")]
        [SwaggerOperation(Summary = "Create a new subscription for user")]
        public async Task<IActionResult> CreateSubscriptionForUser([FromBody] SubscriptionRequest request)
        {
            var result = await _subscriptionService.CreateSubscriptionByUserAsync(request);
            return Ok(ApiResponse<SubscriptionResponseV1>.OkResponse(result, "Subscription created successfully!", "201"));
        }

        [HttpPut("subscriptions/{id}")]
        [SwaggerOperation(Summary = "Update a subscription by ID for user")]
        public async Task<IActionResult> UpdateSubscriptionForUser([FromRoute] string id, [FromBody] SubscriptionRequest request)
        {
            var result = await _subscriptionService.UpdateSubscriptionByUserLoginAsync(id, request);
            return Ok(ApiResponse<SubscriptionResponseV1>.OkResponse(result, "Subscription updated successfully!", "200"));
        }

        [HttpDelete("subscriptions/{id}")]
        [SwaggerOperation(Summary = "Delete a subscription by ID for user")]
        public async Task<IActionResult> DeleteSubscriptionForUser([FromRoute] string id)
        {
            await _subscriptionService.DeleteSubscriptionByUserAsync(id);
            return Ok(ApiResponse<string>.OkResponse("Subscription deleted successfully", "200"));
        }   

    }
}
