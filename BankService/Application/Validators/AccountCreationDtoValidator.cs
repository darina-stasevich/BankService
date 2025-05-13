using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class AccountCreationDtoValidator : AbstractValidator<AccountCreationDto>, IAccountCreationDtoValidator
{
    public AccountCreationDtoValidator()
    {
        RuleFor(x => x.Bank).NotNull().WithMessage("Bank name cannot be null");
        RuleFor(x => x.UserAccountId).NotNull().WithMessage("UserAccount ID cannot be null");
        When(x => x.Type == BankAccountType.Enterprise || x.Type == BankAccountType.Salary, () =>
        {
            RuleFor(x => x.Enterprise).NotNull().WithMessage("Enterprise name cannot be null");
        }).Otherwise(() =>
        {
            RuleFor(x => x.Enterprise).Null().WithMessage("Enterprise name must be null");
        });
        When(x => x.Type == BankAccountType.Deposit, () =>
        {
            RuleFor(x => x.DepositAccountOptionsDto).NotNull().WithMessage("Deposit Account Options cannot be null");
            When(x => x.DepositAccountOptionsDto != null, () =>
            {
                RuleFor(x => x.DepositAccountOptionsDto!.InterestRate).GreaterThan(0).WithMessage("InterestRate must be greater than 0");
                RuleFor(x => x.DepositAccountOptionsDto!.MaturityDate).Must(term => term >= DateTime.Now.AddYears(1)).WithMessage("Maturity Date must be greater than greater that one year");
            });
        }).Otherwise(() =>
        {
            RuleFor(x => x.DepositAccountOptionsDto).Null().WithMessage("Deposit Account Options must be null");
        });
    }
}