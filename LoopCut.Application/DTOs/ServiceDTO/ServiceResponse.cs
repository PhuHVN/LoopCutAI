using System.Text.Json.Serialization;
using LoopCut.Application.DTOs.ServicePlanDTO;
using LoopCut.Domain.Enums;

namespace LoopCut.Application.DTOs.ServiceDTO
{
    public class ServiceResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ServiceEnums Status { get; set; }
        public string? ModifiedByName { get; set; }

        public List<ServicePlanResponse>? ServicePlans { get; set; }
    }
}
