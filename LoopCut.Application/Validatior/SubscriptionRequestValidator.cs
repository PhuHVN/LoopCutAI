using FluentValidation;
using LoopCut.Application.DTOs.SubscriptionDTO;

namespace LoopCut.Application.Validatior
{
    public class SubscriptionRequestValidator : AbstractValidator<SubscriptionRequest>
    {
        public SubscriptionRequestValidator() { 
        
            RuleFor(x => x.SubscriptionsName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Subscription name is required.")
                .MaximumLength(100).WithMessage("Subscription name cannot exceed 100 characters.");
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate) 
                .WithMessage("End date must be after the start date.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0.0).WithMessage("Price must be a non-negative value.");
            RuleFor(x => x.RemiderDays)
                .GreaterThanOrEqualTo(1).WithMessage("Reminder days must be greater or equal 1");
        }
    }
}
