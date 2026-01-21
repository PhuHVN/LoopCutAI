using LoopCut.Domain.Enums;

namespace LoopCut.Domain.Entities;

public class Services
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; }
    public ServiceEnums Status { get; set; } = ServiceEnums.Active;
    public string? ModifiedByID { get; set; }

    // Navigation Properties
    public required Accounts? ModifiedBy { get; set; }

    public ICollection<ServicePlans> ServicePlans { get; set; } = new List<ServicePlans>();
}
