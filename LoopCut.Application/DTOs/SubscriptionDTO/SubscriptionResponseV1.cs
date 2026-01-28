using System.Text.Json.Serialization;
using LoopCut.Application.DTOs.ServicePlanDTO;
using LoopCut.Domain.Enums;

namespace LoopCut.Application.DTOs.SubscriptionDTO
{
    public class SubscriptionResponseV1
    {
        public string? Id { get; set; }
        public string? ServicePlanId { get; set; }
        public string SubscriptionsName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public double Price { get; set; } = 0.0;
        public int RemiderDays { get; set; } = 0;

        public SubscriptionEnums Status { get; set; } = SubscriptionEnums.Active;

        public string? PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public double PlanPrice { get; set; } = 0.0;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BillingCycleEnums BillingCycleEnums { get; set; }

        public string? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
}
