using FluentValidation;
using LoopCut.Application.DTOs.ServiceDTO;

namespace LoopCut.Application.Validatior
{
    public class ServicePlanRequestV1Validator : AbstractValidator<ServicePlanRequestV1>
    {
        public ServicePlanRequestV1Validator()
        {
            RuleFor(x => x.PlanName) 
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Plan name is required.")
                .MaximumLength(100).WithMessage("Plan name must not exceed 100 characters.");
            RuleFor(x => x.Price)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");
            RuleFor(x => x.BillingCycleEnums)
                .IsInEnum().WithMessage("Billing cycle must be a valid enum value.");

        }
    }
}
