using BankService.Application.Validators;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.ApprovalServices;

public interface IBankAccountApprovalService
{
    public Result<Guid> ChangeStatusBankAccount(Guid bankAccountId, Guid approverUserAccountId, string bankName,
        BankAccountStatus newStatus);
}