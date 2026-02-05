using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.UserMembershipDtos
{
    public class UserMembershipDetail
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public RoleEnum Role { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public MembershipDetail Membership { get; set; } = new MembershipDetail();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class MembershipDetail
    {
        public string MembershipId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInMonths { get; set; }
        public decimal Price { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
    }
}
