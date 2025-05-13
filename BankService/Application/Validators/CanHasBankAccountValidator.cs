using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class CanHasBankAccountValidator : AbstractValidator<UserAccount>, ICanHasBankAccountValidator
{
    public CanHasBankAccountValidator()
    {
        RuleFor(x => x.Status).Must(status => status == VerificationStatus.Approved).WithMessage("Only active accounts can has bank accounts");
        RuleFor(x => x.UserRole).Must(role => (role == UserRole.Client || role == UserRole.ExternalSpecialist)).WithMessage("Only clients can have bank accounts");
    }
}