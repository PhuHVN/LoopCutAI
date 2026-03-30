using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.DTOs.FilterLogDtos;
using LoopCut.Application.DTOs.MembershipDtos;
using LoopCut.Application.DTOs.PaymentDTO;
using LoopCut.Application.DTOs.SubscriptionDTO;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogService _logService;
        private readonly IAccountService _accountService;
        private readonly IMembershipService _membershipService;
        private readonly ISubscriptionService _subscriptionService;

        public AdminController(
            IPaymentService paymentService, 
            ILogService logService, 
            IAccountService accountService,
            IMembershipService membershipService,
            ISubscriptionService subscriptionService)
        {
            _paymentService = paymentService;
            _logService = logService;
            _accountService = accountService;
            _membershipService = membershipService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet("payment-history")]
        public async Task<IActionResult> GetPaymentHistory(int pageIndex = 1, int pageSize = 10,[FromQuery] PaymentFilterRequest request = null)
        {
            var result = await _paymentService.GetAllPayments(pageIndex, pageSize, request);
            return Ok(ApiResponse<BasePaginatedList<PaymentDetailResponse>>.OkResponse(result, "Create account successful!", "201"));
        }
        [HttpGet("user-activity-logs")]
        public async Task<IActionResult> GetUserActivityLogs(int pageIndex = 1, int pageSize = 10,[FromQuery] FilterLogDto request = null)
        {
            var result = await _logService.GetLogsByFilterAsync(pageIndex, pageSize, request);
            return Ok(ApiResponse<BasePaginatedList<LogResponse>>.OkResponse(result, "Get user activity logs successful!", "200"));
        }
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAllAccounts(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _accountService.GetAllAccounts(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<AccountResponse>>.OkResponse(result, "Get all accounts successful!", "200"));
        }

        [HttpGet("memberships")]
        public async Task<IActionResult> GetAllMemberships(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _membershipService.GetAllMemberships(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<MembershipResponse>>.OkResponse(result, "Get all memberships successful!", "200"));
        }

        [HttpGet("memberships/{id}")]
        public async Task<IActionResult> GetMembershipById(string id)
        {
            var result = await _membershipService.GetMembershipById(id);
            return Ok(ApiResponse<MembershipResponse>.OkResponse(result, "Get membership successful!", "200"));
        }

        [HttpPost("memberships")]
        public async Task<IActionResult> CreateMembership(MembershipRequest request)
        {
            var result = await _membershipService.CreateMembership(request);
            return Ok(ApiResponse<MembershipResponse>.OkResponse(result, "Create membership successful!", "201"));
        }

        [HttpPut("memberships/{id}")]
        public async Task<IActionResult> UpdateMembership(string id, MembershipUpRes request)
        {
            var result = await _membershipService.UpdateMembership(id, request);
            return Ok(ApiResponse<MembershipResponse>.OkResponse(result, "Update membership successful!", "200"));
        }

        [HttpDelete("memberships/{id}")]
        public async Task<IActionResult> DeleteMembership(string id)
        {
            await _membershipService.DeleteMembership(id);
            return Ok(ApiResponse<object>.OkResponse(null, "Delete membership successful!", "200"));
        }

        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetAllSubscriptions(int pageIndex = 1, int pageSize = 10, string subName = "", int reminderDays = 0, string serviceName = "")
        {
            var result = await _subscriptionService.GetAllSubscriptionsWithPlanAndServiceByManagerAsync(pageIndex, pageSize, subName, reminderDays, serviceName);
            return Ok(ApiResponse<BasePaginatedList<SubscriptionResponseV2>>.OkResponse(result, "Get all subscriptions successful!", "200"));
        }

        [HttpGet("subscriptions/{id}")]
        public async Task<IActionResult> GetSubscriptionById(string id)
        {
            var result = await _subscriptionService.GetSubscriptionWithPlanAndServiceByIdByManagerAsync(id);
            return Ok(ApiResponse<SubscriptionResponseV2>.OkResponse(result, "Get subscription successful!", "200"));
        }

        [HttpDelete("subscriptions/{id}")]
        public async Task<IActionResult> CancelSubscription(string id)
        {
            await _subscriptionService.DeleteSubscriptionByUserAsync(id);
            return Ok(ApiResponse<object>.OkResponse(null, "Cancel subscription successful!", "200"));
        }
        [HttpPost("accounts")]
        public async Task<IActionResult> CreateAccount(AccountRequest request)
        {
            var result = await _accountService.CreateAccount(request);
            return Ok(ApiResponse<AccountResponse>.OkResponse(result, "Create account successful!", "201"));
        }

        [HttpPut("accounts/{id}")]
        public async Task<IActionResult> UpdateAccount(string id, AccountUpRequest request)
        {
            var result = await _accountService.UpdateAccount(id, request);
            return Ok(ApiResponse<AccountResponse>.OkResponse(result, "Update account successful!", "200"));
        }

        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            await _accountService.DeleteAccount(id);
            return Ok(ApiResponse<object>.OkResponse(null, "Delete account successful!", "200"));
        }
        [HttpGet("statistics/dashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var payments = await _paymentService.GetAllPayments(1, 1);
                var accounts = await _accountService.GetAllAccounts(1, 1);
                var subscriptions = await _subscriptionService.GetAllSubscriptionsByUserLoginAsync(1, 1);
                var logs = await _logService.GetLogsByFilterAsync(1, 1, null);

                var statistics = new
                {
                    totalPayments = payments?.TotalCount ?? 0,
                    totalAccounts = accounts?.TotalCount ?? 0,
                    totalSubscriptions = subscriptions?.TotalCount ?? 0,
                    totalLogs = logs?.TotalCount ?? 0,
                    timestamp = DateTime.UtcNow
                };

                return Ok(ApiResponse<object>.OkResponse(statistics, "Dashboard statistics retrieved successfully!", "200"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.BadResponse("Error retrieving dashboard statistics: " + ex.Message, "400"));
            }
        }
    }
}
