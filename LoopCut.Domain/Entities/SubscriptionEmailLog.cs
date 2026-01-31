using LoopCut.Domain.Enums;

namespace LoopCut.Domain.Entities
{
    public class SubscriptionEmailLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SubscriptionId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public SubscriptionEmailType Type { get; set; }
        // PreReminder | DailyReminder | Expired

        public DateTime SentAt { get; set; }
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        // Navigation
        public Subscriptions Subscription { get; set; } = null!;
    }
}
