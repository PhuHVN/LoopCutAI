using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Entities
{
    public class Payment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string MembershipId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public PaymentStatusEnum Status { get; set; } = PaymentStatusEnum.Pending;


    }
}
