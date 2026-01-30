using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.PaymentDTO
{
    public class PaymentRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string MembershipId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
