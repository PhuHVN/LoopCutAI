using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.PaymentDTO;
using LoopCut.Application.DTOs.PayOsDto;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayOS.Models.Webhooks;
using PayOS.Resources.Webhooks;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService payment;
        public PaymentController(IPaymentService payment)
        {
            this.payment = payment;
        }
        [HttpPost("create_payment_link")]
        [SwaggerOperation(Summary = "Create a payment link for a membership purchase.")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequest request)
        {
            var result = await payment.CreatePaymentLink(request);
            return Ok(result);
        }
        [HttpPost("webhook")]
        [SwaggerOperation(Summary = "[DON''T USE THIS API] Handle payment webhook notifications from PayOS.")]
        public async Task<IActionResult> Webhook([FromBody] Webhook body)
        {
            var isSuccess = await payment.VerifyPaymentWebhook(body);
            if (isSuccess)
            {
                return Ok(new { success = true, message = "Webhook processed successfully" });
            }
            else
            {
                return Ok(new { success = false, message = "Webhook processing failed" });
            }
        }

        [HttpGet("success")]
        [SwaggerOperation(Summary = "[DON''T USE THIS API] Get payment success status.")]
        public IActionResult Success([FromQuery] long orderCode)
        {
            return Ok(new
            {
                message = "Payment completed. Please check status via API.",
                orderCode = orderCode
            });
        }

        [HttpGet("cancel")]
        [SwaggerOperation(Summary = "[DON''T USE THIS API] Get payment cancellation status.")]
        public IActionResult Cancel([FromQuery] long orderCode)
        {
            return Ok(new
            {
                message = "Payment cancelled.",
                orderCode = orderCode
            });
        }
        [HttpGet("{orderCode}")]
        [SwaggerOperation(Summary = "Get payment information by order code.")]
        public async Task<IActionResult> GetByOrderCode([FromRoute] string orderCode)
        {
            var rs = await payment.GetPaymentInfo(orderCode);
            return Ok(ApiResponse<PaymentDetailResponse>.OkResponse(rs, "Get successful!", "200"));
        }

        [HttpGet("history")]
        [SwaggerOperation(Summary = "Get paginated list of payments with optional filters.")]
        public async Task<IActionResult> GetAllPayments([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] PaymentFilterRequest? filter = null)
        {
            var rs = await payment.GetAllPayments(pageIndex, pageSize, filter ?? new PaymentFilterRequest());
            return Ok(ApiResponse<BasePaginatedList<PaymentDetailResponse>>.OkResponse(rs, "Get successful!", "200"));
        }
    }
}
