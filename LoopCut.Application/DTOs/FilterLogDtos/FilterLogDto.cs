using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.FilterLogDtos
{
    public class FilterLogDto
    {
        public string? UserId { get; set; }
        public AuditActionEnum? Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public StatusEnum? Status { get; set; }
        public string? IpAddress { get; set; }

        // Optional: Search trong OldValues/NewValues
        public string? SearchInValues { get; set; }
    }
}
