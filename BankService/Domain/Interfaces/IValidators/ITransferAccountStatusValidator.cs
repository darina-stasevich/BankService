using BankService.Domain.Entities.BankAccounts;
using FluentValidation;

namespace BankService.Domain.Interfaces.IValidators;

public interface ITransferAccountStatusValidator : IValidator<Tuple<BankAccount, BankAccount>>
{
    
}