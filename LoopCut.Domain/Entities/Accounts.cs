using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Entities
{
    public class Accounts
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;              
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public RoleEnum Role { get; set; } 
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        //Navigation Properties
        public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    }
}
