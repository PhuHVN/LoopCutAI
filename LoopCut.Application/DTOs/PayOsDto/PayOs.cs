using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.PayOsDto
{
    public class CreatePaymentRequest
    {
        public string MembershipId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? ReturnUrlDomain { get; set; }

    }

    public class PaymentResponsed
    {
        public string CheckoutUrl { get; set; }
        public long OrderCode { get; set; }
        public string Message { get; set; }
    }
    public class PayOSWebhookRequest
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }

        public PayOSWebhookData Data { get; set; }

        public string Signature { get; set; }
    }

    public class PayOSWebhookData
    {
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string TransactionDateTime { get; set; }
    }
}
