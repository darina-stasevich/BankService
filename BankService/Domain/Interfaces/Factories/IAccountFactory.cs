using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces;

public interface IAccountFactory
{
    public Result<BankAccount> CreateAccount(AccountCreationDto dto);
}