using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class BankAccountValidator : AbstractValidator<BankAccount>, IBankAccountValidator
{
    public BankAccountValidator()
    {
        RuleFor(x => x.Balance).GreaterThanOrEqualTo(0);
        When(x => x.Type == BankAccountType.Salary && x is SalaryAccount, () =>
        {
            RuleFor(x => (x as SalaryAccount)!.EnterpriseId)
                .NotEmpty().WithMessage("Enterprise account cannot be empty");
        });
        When(x => x.Type == BankAccountType.Enterprise && x is EnterpriseAccount, () =>
        {
            RuleFor(x => (x as EnterpriseAccount)!.EnterpriseId)
                .NotEmpty().WithMessage("Enterprise must be specified");
        });
    }
}