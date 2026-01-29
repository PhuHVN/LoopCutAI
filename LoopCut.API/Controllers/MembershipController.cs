using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.MembershipDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/memberships")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new membership")]
        public async Task<IActionResult> CreateMembership([FromBody] MembershipRequest membershipRequest)
        {
            var result = await _membershipService.CreateMembership(membershipRequest);
            return Ok(ApiResponse<MembershipResponse>.OkResponse(result, "Create successful!", "200"));
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a membership by ID")]
        public async Task<IActionResult> DeleteMembership([FromRoute] string id)
        {
            await _membershipService.DeleteMembership(id);
            return NoContent();
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get all memberships with pagination")]
        public async Task<IActionResult> GetAllMemberships([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _membershipService.GetAllMemberships(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<MembershipResponse>>.OkResponse(result, "Fetch successful!", "200"));
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a membership by ID")]
        public async Task<IActionResult> GetMembershipById([FromRoute] string id)
        {
            var result = await _membershipService.GetMembershipById(id);
            return Ok(ApiResponse<MembershipResponse>.OkResponse(result, "Fetch successful!", "200"));

        }
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a membership by ID")]
        public async Task<IActionResult> UpdateMembership([FromRoute] string id, [FromBody] MembershipUpRes membershipRequest)
        {
            var result = await _membershipService.UpdateMembership(id, membershipRequest);
            return Ok(ApiResponse<MembershipResponse>.OkResponse(result, "Update successful!", "200"));
        }
    }
}
