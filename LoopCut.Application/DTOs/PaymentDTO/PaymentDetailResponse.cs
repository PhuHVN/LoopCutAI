using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.PaymentDTO
{
    public class PaymentDetailResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MembershipId { get; set; } = string.Empty;
        public string MembershipName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public PaymentStatusEnum Status {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
