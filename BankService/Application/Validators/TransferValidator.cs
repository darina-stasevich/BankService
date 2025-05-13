using BankService.Domain.Entities.DTOs;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class TransferValidator : AbstractValidator<TransferRequest>, ITransferValidator
{
    public TransferValidator()
    {
        RuleFor(x => x.ReceiverAccountId).NotEmpty().WithMessage("Receiver account is required");
        RuleFor(x => x.SenderAccountId).NotEmpty().WithMessage("Sender account is required");
        RuleFor(x => x.ReceiverAccountId).NotEqual(x => x.SenderAccountId)
            .WithMessage("Sender account and receiver account must be different");
    }
}