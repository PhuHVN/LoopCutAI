using LoopCut.Domain.Enums;

namespace LoopCut.Domain.Entities
{
    public class ServicePlans
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string ServiceId { get; set; }
        public string? ModifiedByID { get; set; }

        public string PlanName { get; set; } = string.Empty;

        public double Price { get; set; } = 0.0;

        public BillingCycleEnums BillingCycleEnums { get; set; } = BillingCycleEnums.Monthly;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdatedAt { get; set; }

        // Navigation Properties
        public required Accounts? ModifiedBy { get; set; }

        public required Services Services { get; set; }

        public ICollection<Subscriptions> Subcriptions { get; set; } = new List<Subscriptions>();
    }
}
