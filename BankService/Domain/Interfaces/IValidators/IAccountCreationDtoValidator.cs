using BankService.Domain.Entities.DTOs;
using FluentValidation;

namespace BankService.Domain.Interfaces.IValidators;

public interface IAccountCreationDtoValidator : IValidator<AccountCreationDto>
{
    
}