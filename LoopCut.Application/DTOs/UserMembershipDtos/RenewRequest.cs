using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.UserMembershipDtos
{
    public class RenewRequest
    {
        public string UserId { get; set; } = string.Empty;       
        public int MonthAmount { get; set; }
    }
}
