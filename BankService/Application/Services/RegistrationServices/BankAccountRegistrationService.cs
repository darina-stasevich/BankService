using System.Runtime.InteropServices.JavaScript;
using BankService.Application.Validators;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.IValidators;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Results;
using BankService.Infrastructure;

namespace BankService.Application.Services;

public class BankAccountRegistrationService(
    IEnterpriseRepository enterpriseRepository,
    IBankAccountRepository bankAccountRepository,
    IUserAccountRepository userAccountRepository,
    IBankAccountValidator bankAccountValidator,
    ICanHasBankAccountValidator canHasBankAccountValidator,
    IAccountCreationDtoValidator accountCreationDtoValidator,
    IAccountFactory accountFactory
    ) : IBankAccountRegistrationService
{
    public Result<Guid> CreateBankAccount(string bankName, Guid requestAccountId, AccountCreationDto accountCreationDto)
    {
        var bank = enterpriseRepository.GetByName(bankName);
        if(bank == null)
            return Error.NotFound(400, $"bank with name {bankName} not found");
        var isBank = enterpriseRepository.IsBank(bank.Id);
        if (isBank == false)
            return Error.NotFound(400, $"There is no bank with bank id {bank.Id}");

        var userAccount = userAccountRepository.GetById(requestAccountId, bank.Id);
        if (userAccount == null)
            return Error.NotFound(400, $"There is no user account with id {requestAccountId}");
        
        if (accountCreationDto.Type == BankAccountType.Enterprise)
        {
            if (userAccount.UserRole != UserRole.ExternalSpecialist)
            {
                return Error.AccessForbidden(403, "only specialist can create enterprise account");
            }
            var enterprise = enterpriseRepository.GetById(userAccount.EnterpriseId!.Value);
            if(enterprise == null)
                return Error.NotFound(400, $"request account with id {requestAccountId} not found in enterprises");
            accountCreationDto.Enterprise = enterprise.Name;
            accountCreationDto.UserAccountId = requestAccountId;
        }
        else
        {
            
            var resultValidateUserAccount = canHasBankAccountValidator.Validate(userAccount);
            if (!resultValidateUserAccount.IsValid)
            {
                return Error.Validation(400, string.Join(" ", resultValidateUserAccount.Errors.Select(e => e.ErrorMessage)));
            }

            if (accountCreationDto.Type == BankAccountType.Enterprise &&
                userAccount.UserRole != UserRole.ExternalSpecialist)
            {
                return Error.Validation(400, "enterprise accounts can open only specialists");
            }

            if (userAccount.UserRole == UserRole.ExternalSpecialist &&
                accountCreationDto.Type != BankAccountType.Enterprise)
            {
                return Error.Validation(400, "enterprise specialists can open only enterprise accounts");
            }
            accountCreationDto.UserAccountId = requestAccountId;
        }

        accountCreationDto.Bank = bankName;
        
        var result = accountCreationDtoValidator.Validate(accountCreationDto);
        if(!result.IsValid)
            return Error.Validation(400, string.Join(" ", result.Errors.Select(e => e.ErrorMessage)));
        
        var accountCreationResult = accountFactory.CreateAccount(accountCreationDto); 
        if (!accountCreationResult.IsSuccess)
            return accountCreationResult.Error;
        var bankAccount = accountCreationResult.Value;
       
        var resultCreateAccount = bankAccountValidator.Validate(bankAccount);
        if (resultCreateAccount.IsValid)
        {
            // validation completed
            bankAccountRepository.Add(bankAccount);
            return bankAccount.Id;
        }
        return Error.Validation(400, "validation of bank account failed");
    }
}