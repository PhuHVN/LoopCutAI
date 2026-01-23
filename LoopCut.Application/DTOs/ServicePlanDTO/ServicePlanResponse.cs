using System.Text.Json.Serialization;
using LoopCut.Domain.Enums;

namespace LoopCut.Application.DTOs.ServicePlanDTO
{
    public class ServicePlanResponse
    {
        public string? Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BillingCycleEnums BillingCycleEnums { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServicePlanEnums Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public string? ModifiedByName { get; set; }
    }
}
