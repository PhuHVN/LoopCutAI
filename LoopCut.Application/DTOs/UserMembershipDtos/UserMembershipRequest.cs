using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.UserMembershipDtos
{
    public class UserMembershipRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string MembershipId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
    }
}
