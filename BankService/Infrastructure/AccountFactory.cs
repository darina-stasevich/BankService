using System.ComponentModel.DataAnnotations;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Results;

namespace BankService.Infrastructure;

public class AccountFactory(
    IEnterpriseRepository enterpriseRepository) : IAccountFactory
{
    IAccountBuilder _builder;

    public Result<BankAccount> CreateAccount(AccountCreationDto dto)
    {
        var bank = enterpriseRepository.GetByName(dto.Bank);
        var enterprise = enterpriseRepository.GetByName(dto.Enterprise);
        if(!enterpriseRepository.IsBank(dto.Bank))
            return Error.Failure(400, "Bank doesn't exist");
        if(bank == null)
            return Error.Failure(400, "Enterprise doesn't exist");
        if((dto.Type == BankAccountType.Salary || dto.Type == BankAccountType.Enterprise) && enterprise == null)
            return Error.Failure(400, "Enterprise doesn't exist");
        try
        {
            _builder = new AccountBuilder().WithBankId(bank.Id).WithBankAccountType(dto.Type);
            Console.WriteLine($"Fabric {dto.Type}");
            switch (dto.Type)
            {
                case BankAccountType.Deposit:
                {
                    _builder.WithDepositOptions(dto.DepositAccountOptionsDto);
                    _builder.WithUserAccountId(dto.UserAccountId.Value);
                    break;
                }
                case BankAccountType.Salary:
                {
                    _builder.WithEnterpriseId(enterprise.Id);
                    _builder.WithUserAccountId(dto.UserAccountId.Value);
                    break;
                }
                case BankAccountType.Enterprise:
                {
                    _builder.WithEnterpriseId(enterprise.Id);
                    _builder.WithUserAccountId(dto.UserAccountId.Value);
                    break;
                }
                case BankAccountType.Current:
                {
                    _builder.WithUserAccountId(dto.UserAccountId.Value);
                    break;
                }
            }

            return _builder.Build();
        }
        catch (Exception e)
        {
            return Error.Failure(400, e.Message);
        }
    }
}