using FluentValidation;
using LoopCut.Application.DTOs.AccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Validatior
{
    public class AccountRequestValidatior : AbstractValidator<AccountRequest>
    {
        public AccountRequestValidatior()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");
            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
            //RuleFor(x => x.PhoneNumber)
            //    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            //    .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}
