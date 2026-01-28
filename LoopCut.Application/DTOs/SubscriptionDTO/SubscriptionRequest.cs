namespace LoopCut.Application.DTOs.SubscriptionDTO
{
    public class SubscriptionRequest
    {
        public string? ServicePlanId { get; set; } // Can be null if subscription is not linked to a specific service plan in the system
        public string SubscriptionsName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public double Price { get; set; } = 0.0;
        public int RemiderDays { get; set; } = 0;
    }
}
