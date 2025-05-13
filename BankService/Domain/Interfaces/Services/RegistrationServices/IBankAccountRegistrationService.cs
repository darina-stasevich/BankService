using BankService.Domain.Entities.DTOs;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.RegistrationServices;

public interface IBankAccountRegistrationService
{
    public Result<Guid> CreateBankAccount(string bankName, Guid requestAccountId, AccountCreationDto accountCreationDto);
}