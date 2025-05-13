using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class WithdrawAccountValidator : AbstractValidator<BankAccount>, IWithdrawAccountValidator
{
    public WithdrawAccountValidator()
    {
        RuleFor(x => x.Status).Equal(BankAccountStatus.Active).WithMessage("Bank account status must be Active");
        RuleFor(x => x.Type).Must(type => type != BankAccountType.Enterprise)
            .WithMessage("Enterprise account not support withdrawal");
        When(x => x.Type == BankAccountType.Deposit, () =>
        {
            RuleFor(x => (DepositAccount)x).Must(x => x.IsEarlyWithdrawalAllowed = true)
                .WithMessage("Deposit account is not early withdrawal");
        });
    }
}