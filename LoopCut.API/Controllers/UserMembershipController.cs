using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.UserMembershipDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/user-membership")]
    [ApiController]
    public class UserMembershipController : ControllerBase
    {
        private readonly IUserMembershipService _service;
        public UserMembershipController(IUserMembershipService service)
        {
            _service = service;
        }

        [HttpPost("assign")]
        [SwaggerOperation(Summary = "Assign a membership to a user")]
        public async Task<IActionResult> AssignUserToMembership([FromBody] UserMembershipRequest request)
        {
            var result = await _service.AssignMembershipToUser(request);
            return Ok(ApiResponse<UserMembershipResponse>.OkResponse(result, "User assigned to membership successfully", "200"));
        }

        [HttpPatch("expire_membership")]
        [SwaggerOperation(Summary = "Expire a user's membership")]
        public async Task<IActionResult> ExpireUserMembership(string userId, string membershipId)
        {
            var result = await _service.ExpireMembershipFromUser(userId, membershipId);
            return Ok(ApiResponse<UserMembershipResponse>.OkResponse(result, "User membership expired successfully", "200"));
        }
        [HttpPatch("activate_membership")]
        [SwaggerOperation(Summary = "Activate a user's membership")]
        public async Task<IActionResult> ActiveUserMembership(string userId, string membershipId)
        {
            var result = await _service.ActiveMembershipFromUser(userId, membershipId);
            return Ok(ApiResponse<UserMembershipResponse>.OkResponse(result, "User membership activated successfully", "200"));
        }

        [HttpGet("get_user_memberships")]
        [SwaggerOperation(Summary = "Get all user memberships with pagination")]
        public async Task<IActionResult> GetUserMemberships(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _service.GetUserMemberships(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<UserMembershipResponse>>.OkResponse(result, "User memberships retrieved successfully", "200"));
        }

        [HttpPut("update_membership")]
        [SwaggerOperation(Summary = "Update a user's membership details")]
        public async Task<IActionResult> UpdateUserMembership([FromBody] UserMembershipRequest request)
        {
            var result = await _service.UpdateMembershipToUser(request);
            return Ok(ApiResponse<UserMembershipResponse>.OkResponse(result, "User membership updated successfully", "200"));
        }
    }
}
