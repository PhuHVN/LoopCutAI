using System.Text.Json.Serialization;
using LoopCut.Domain.Enums;

namespace LoopCut.Application.DTOs.ServiceDTO
{
    public class ServiceRequestV1
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;

        public List<ServicePlanRequestV1>? ServicePlans { get; set; } 
    }

    public class ServicePlanRequestV1
    {
        public string PlanName { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;

        public BillingCycleEnums? BillingCycleEnums { get; set; }
    }
}
