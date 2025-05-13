using BankService.Domain.Entities.DTOs;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class WithdrawValidator : AbstractValidator<TransferRequest>, IWithdrawValidator
{
    public WithdrawValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Minimal amount for withdrawal must be greater than 0");
    }
}