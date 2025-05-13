using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class TransferAccountStatusValidator : AbstractValidator<Tuple<BankAccount, BankAccount>>, ITransferAccountStatusValidator
{
    public TransferAccountStatusValidator()
    {
        RuleFor(t => t.Item1.Status).Equal(BankAccountStatus.Active).WithMessage("Invalid sender bank account status");
        RuleFor(t => t.Item2.Status)
            .Must(status => status == BankAccountStatus.Active || status == BankAccountStatus.Freezed)
            .WithMessage("Invalid receiver bank account status");
        RuleFor(t => t.Item1.Type).Must(type => type != BankAccountType.Deposit)
            .WithMessage("This type of bank account does not support transfer operations");
        When(t => t.Item1.Type == BankAccountType.Enterprise, () =>
        {
            RuleFor(r => r.Item2.Type).Must(type => type == BankAccountType.Salary)
                .WithMessage("Enterprise account can perform transfer only to salary accounts");
        });
        When(t => t.Item2.Type == BankAccountType.Salary, () =>
        {
            RuleFor(r => r.Item1.Type).Must(type => type == BankAccountType.Enterprise)
                .WithMessage("only enterprise account can perform transfer to salary account");
        });
    }
}