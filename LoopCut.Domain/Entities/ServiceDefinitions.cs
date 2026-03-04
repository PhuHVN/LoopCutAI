using LoopCut.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoopCut.Domain.Entities;

public class ServiceDefinitions
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; }
    public ServiceEnums Status { get; set; } = ServiceEnums.Active;
    public string? ModifiedById { get; set; }

    // Navigation Properties
    public  Accounts? ModifiedBy { get; set; }

    public ICollection<ServicePlans> ServicePlans { get; set; } = new List<ServicePlans>();
}
