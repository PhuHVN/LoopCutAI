using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Entities
{
    public class Membership
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } 

        public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    }
}
