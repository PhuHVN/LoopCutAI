using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Entities
{
    public class AuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public AuditActionEnum Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? OldValues { get; set; } = string.Empty;
        public string? NewValues { get; set; } = string.Empty;
        public string? IpAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;

    }
}
