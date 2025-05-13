using BankService.Domain.Entities;
using FluentValidation;

namespace BankService.Domain.Interfaces.IValidators;

public interface IUserValidator : IValidator<User>
{
    
}