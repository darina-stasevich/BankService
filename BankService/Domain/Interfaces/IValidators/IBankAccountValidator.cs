using BankService.Domain.Entities.BankAccounts;
using FluentValidation;

namespace BankService.Domain.Interfaces.IValidators;

public interface IBankAccountValidator : IValidator<BankAccount>
{
    
}