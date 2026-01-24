using FluentValidation;
using LoopCut.Application.DTOs.ServiceDTO;

namespace LoopCut.Application.Validatior
{
    public class ServiceRequestV1Validator : AbstractValidator<ServiceRequestV1>
    {
        public ServiceRequestV1Validator()
        {

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Service name is required.")
                .MaximumLength(100).WithMessage("Service name must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Service description must not exceed 500 characters.");
            RuleFor(x => x.LogoUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .When(x => !string.IsNullOrEmpty(x.LogoUrl))
                .WithMessage("Logo URL must be a valid URL.");
        }
    }
}
