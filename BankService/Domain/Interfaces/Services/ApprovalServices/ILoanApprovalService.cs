using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.ApprovalServices;

public interface ILoanApprovalService
{
    public Result<Guid> ApproveLoan(Guid loanRequestId, Guid approverAccountId, string bankName);
    public Result RejectLoan(Guid loanRequestId, Guid approverAccountId, string bankName);
}