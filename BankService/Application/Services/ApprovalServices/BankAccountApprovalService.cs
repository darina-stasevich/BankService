using BankService.Application.Validators;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class BankAccountApprovalService(
    IUserAccountRepository userAccountRepository,
    IBankAccountRepository bankAccountRepository,
    IEnterpriseRepository enterpriseRepository
) : IBankAccountApprovalService
{
    public Result<Guid> ChangeStatusBankAccount(Guid bankAccountId, Guid approverUserAccountId, string bankName, BankAccountStatus newStatus)
    {
        if (!enterpriseRepository.IsBank(bankName))
            return Error.NotFound(400, $"bank {bankName} not found");
        var bank = enterpriseRepository.GetByName(bankName);
        var approver = userAccountRepository.GetById(approverUserAccountId, bank!.Id);
        if (approver == null)
            return Error.AccessUnAuthorized(400, $"There is no account with id {approverUserAccountId}");
        var role = approver.UserRole;

        var bankAccount = bankAccountRepository.GetById(bankAccountId, bank!.Id);
        if (bankAccount == null)
            return Error.NotFound(400, $"Bank account {bankAccountId} not found.");

        if (bankAccount.Status == newStatus)
            return Error.Conflict(400,
                $"Bank account {bankAccountId} already has status {newStatus}.");

        if (!RolePermissionValidator.CanApproveStatus(role, bankAccount.Status, newStatus))
            return Error.AccessForbidden(403,
                $"Not enough permission to change status of bank account with id {bankAccountId}.");
        
        if (bankAccount.Status == BankAccountStatus.Pending &&
            (newStatus != BankAccountStatus.Active && newStatus != BankAccountStatus.Rejected))
            return Error.Conflict(400, $"pending status can be changed to active or rejected only");
        bankAccount.Status = newStatus;
        if (newStatus == BankAccountStatus.Freezed)
            bankAccount.FrozenTill = DateTime.Now.AddHours(24);
        else
            bankAccount.FrozenTill = null;

        if (newStatus == BankAccountStatus.Blocked)
        {
            bankAccount.BlockedDate = DateTime.Now;
        }
        bankAccountRepository.Update(bankAccount);
        return bankAccount.BankId;
    }
}