using LoopCut.Domain.Enums;

namespace LoopCut.Domain.Entities;

public class Subscriptions
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AccountId { get; set; } = string.Empty;
    public string? ServicePlanId { get; set; } // Can be null if subscription is not linked to a specific service plan in the system
    public string SubscriptionsName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public double Price { get; set; } = 0.0;
    public int RemiderDays { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; }
    public SubscriptionEnums Status { get; set; } = SubscriptionEnums.Active;

    // Navigation Properties
    public required Accounts? Account { get; set; }

    public ServicePlans? ServicePlans { get; set; }
}
