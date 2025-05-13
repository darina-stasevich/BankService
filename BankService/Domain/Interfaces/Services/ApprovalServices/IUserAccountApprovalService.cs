using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.ApprovalServices;

public interface IUserAccountApprovalService
{
    public Result<Guid> ApproveAccount(Guid accountId, Guid approverUserAccountId, string approverBankName);
    public Result<Guid> RejectAccount(Guid accountId, Guid approverUserAccountId, string approverBankName);

}