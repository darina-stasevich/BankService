using System.Diagnostics;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Results;

namespace BankService.Infrastructure;

public class AccountBuilder : IAccountBuilder
{
    private Guid _userAccountId;
    private Guid _bankId;
    private Guid _enterpriseId;
    private BankAccountType _bankAccountType;
    private DepositAccountOptionsDto _depositAccountOptionsDto;


    private void Init(BankAccount account)
    {
        account.UserAccountId = _userAccountId;
        account.BankId = _bankId;
        account.Status = BankAccountStatus.Pending;
    }
    
    public Result<BankAccount> Build()
    {
        Console.WriteLine($"Builder {_bankAccountType}");
        try
        {
            
            switch(_bankAccountType)
            {
                case BankAccountType.Current:
                {
                    var account = new CurrentAccount {};
                    Init(account);
                    return account;
                }
                case BankAccountType.Deposit:
                {
                    var account = new DepositAccount
                    {
                        InterestRate = _depositAccountOptionsDto.InterestRate,
                        IsEarlyWithdrawalAllowed = _depositAccountOptionsDto.IsEarlyWithdrawalAllowed,
                        MaturityDate = _depositAccountOptionsDto.MaturityDate
                    };
                    Init(account);      
                    return account;
                }
                case BankAccountType.Salary:
                {
                    var account = new SalaryAccount
                    {
                        EnterpriseId = _enterpriseId
                    };
                    Init(account);
                    return account;
                }
                case BankAccountType.Enterprise:
                {
                    var account = new EnterpriseAccount
                    {
                        EnterpriseId = _enterpriseId,
                    };
                    Init(account);
                    return account;
                }
                    default:
                    return Error.Failure(400, $"type {_bankAccountType} not supported");
            };
        }
        catch (Exception e)
        {
            return Error.Failure(400, e.Message);
        }
    }

    public IAccountBuilder WithUserAccountId(Guid userAccountId)
    {
        _userAccountId = userAccountId;
        return this;
    }

    public IAccountBuilder WithBankId(Guid bankId)
    {
        _bankId = bankId;
        return this;
    }

    public IAccountBuilder WithEnterpriseId(Guid enterpriseId)
    {
        _enterpriseId = enterpriseId;
        return this;
    }

    public IAccountBuilder WithBankAccountType(BankAccountType bankAccountType)
    {
        _bankAccountType = bankAccountType;
        return this;
    }

    public IAccountBuilder WithDepositOptions(DepositAccountOptionsDto optionsDto)
    {
        _depositAccountOptionsDto = optionsDto;
        return this;
    }
}