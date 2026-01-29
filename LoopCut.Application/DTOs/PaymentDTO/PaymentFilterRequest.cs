using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.PaymentDTO
{
    public class PaymentFilterRequest
    {
        public string? UserId { get; set; }
        public PaymentStatusEnum? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? MembershipId { get; set; }
        public string? OrderCode { get; set; }
        public string? Email { get; set; }
    }
}
