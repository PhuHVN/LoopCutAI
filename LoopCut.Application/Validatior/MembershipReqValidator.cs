using FluentValidation;
using LoopCut.Application.DTOs.MembershipDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Validatior
{
    public class MembershipReqValidator : AbstractValidator<MembershipRequest>
    {
        public MembershipReqValidator() { 
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");

        }
    }
}
