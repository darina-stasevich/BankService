using System.Runtime.InteropServices.JavaScript;
using BankService.Application.Generators;
using BankService.Application.Validators;
using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Results;
using BankService.Infrastructure.Repositories;

namespace BankService.Application.Services;

public class UserAccountApprovalService(
    IEnterpriseRepository bankRepository,
    IUserRepository userRepository,
    IUserAccountRepository userAccountRepository,
    IPasswordHasher passwordHasher,
    INotificationService notificationService) : IUserAccountApprovalService
{
    private readonly Generator _generator = new();

    private Result<UserAccount> ValidateApprover(Guid userAccountId, Guid bankId)
    {
        var approver = userAccountRepository.GetById(userAccountId, bankId);
        if (approver == null)
            return Error.AccessUnAuthorized(400, $"There is no account with id {userAccountId}");

        var role = approver.UserRole;

        if (!RolePermissionValidator.CanApproveAccount(role))
            return Error.AccessForbidden(403, "Not enough permission to change status of user account.");

        return approver;
    }
    private Result<UserAccount> ValidateAccount(Guid accountId, Guid bankId)
    {
        var account = userAccountRepository.GetById(accountId, bankId);
        if (account == null) return Error.NotFound(400, $"Account {accountId} is not found.");

        if (account.UserId == Guid.Empty)
            return Error.NotFound(400, $"Account {accountId} does not have an active user.");

        return account;
    }
    private Result<User> ValidateUser(Guid userId)
    {
        var user = userRepository.GetById(userId);
        if (user == null) return Error.NotFound(400, $"User {userId} does not exist.");
        
        return user;
    }

    public Result<Guid> ApproveAccount(Guid accountId, Guid approverUserAccountId, string approverBankName)
    {
        var bank = bankRepository.GetByName(approverBankName);
        if(bank == null)
            return Error.NotFound(400, $"Bank {approverBankName} does not exist.");
        if(!bankRepository.IsBank(approverBankName))
            return Error.NotFound(400, $"Bank {approverBankName} does not exist.");
        var approverBank = bankRepository.GetByName(approverBankName);
        var approverValidationResult = ValidateApprover(approverUserAccountId, approverBank.Id);
        if (!approverValidationResult.IsSuccess)
            return approverValidationResult.Error;

        var accountValidationResult = ValidateAccount(accountId, approverBank.Id);
        if (!accountValidationResult.IsSuccess)
            return accountValidationResult.Error;

        var account = accountValidationResult.Value;

        var userValidationResult = ValidateUser(account.UserId);
        if (!userValidationResult.IsSuccess)
            return userValidationResult.Error;

        var user = userValidationResult.Value;
        if (user.Status != VerificationStatus.Verified)
            return Error.Validation(403, $"User {account.UserId} is not verified.");

        if (!bankRepository.IsBank(account.BankId))
            return Error.NotFound(400, $"Account {accountId} does not have a bank.");

        if (account.Status == VerificationStatus.Approved)
        {
            return Error.Conflict(400, "account already approved.");
        }
        var login = _generator.Generate();
        var password = _generator.Generate();
        var hash_password = passwordHasher.HashPassword(password);
        account.Login = login;
        account.Password = hash_password;
        account.Status = VerificationStatus.Approved;
        account.ChangedStatusAt = DateTime.Now;

        userAccountRepository.Update(account);

        notificationService.SendCredentials(user.Email, login, password);
        return account.Id;
    }

    public Result<Guid> RejectAccount(Guid accountId, Guid approverUserAccountId, string approverBankName)
    {
     
        var bank = bankRepository.GetByName(approverBankName);
        if(bank == null)
            return Error.NotFound(400, $"Bank {approverBankName} does not exist.");
        if(!bankRepository.IsBank(approverBankName))
            return Error.NotFound(400, $"Bank {approverBankName} does not exist.");
        var approverBank = bankRepository.GetByName(approverBankName);
        
        var approverValidationResult = ValidateApprover(approverUserAccountId, approverBank.Id);
        if (!approverValidationResult.IsSuccess)
            return approverValidationResult.Error;

        var account = userAccountRepository.GetById(accountId, approverBank.Id);
        if (account == null) return Error.NotFound(400, $"Account {accountId} is not found.");

        if(account.Status == VerificationStatus.Rejected)
            return Error.Conflict(400, "Account already rejected.");

        var userValidationResult = ValidateUser(account.UserId);
        if (!userValidationResult.IsSuccess)
            return userValidationResult.Error;

        account.Status = VerificationStatus.Rejected;
        account.ChangedStatusAt = DateTime.Now;

        var user = userValidationResult.Value;
        userAccountRepository.Update(account);
        notificationService.SendNotification(user.Email, $"Your account {account.Login} is rejected.", null);
        return account.Id;
    }


}