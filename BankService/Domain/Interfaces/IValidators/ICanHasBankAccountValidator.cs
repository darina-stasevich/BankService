using BankService.Domain.Entities;
using BankService.Domain.Entities.BankAccounts;
using FluentValidation;

namespace BankService.Domain.Interfaces.IValidators;

public interface ICanHasBankAccountValidator : IValidator<UserAccount>
{
    
}