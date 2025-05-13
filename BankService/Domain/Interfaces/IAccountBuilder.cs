using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Results;


namespace BankService.Domain.Interfaces;

public interface IAccountBuilder
{
    public Result<BankAccount> Build();
    
    public IAccountBuilder WithUserAccountId(Guid userAccountId);
    public IAccountBuilder WithBankId(Guid bankId);
    public IAccountBuilder WithEnterpriseId(Guid enterpriseId);
    public IAccountBuilder WithBankAccountType (BankAccountType bankAccountType);
    public IAccountBuilder WithDepositOptions(DepositAccountOptionsDto optionsDto);
}