using BankService.Application.Validators;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class LoanApprovalService(
    ILoanRequestRepository loanRequestRepository,
    IBankAccountRepository bankAccountRepository,
    IUserAccountRepository userAccountRepository,
    IEnterpriseRepository bankRepository,
    ILoanRepository loanRepository
) : ILoanApprovalService
{
    private Result ValidateInfo(Guid loanRequestId, Guid approverAccountId, string bankName)
    {
        if (!bankRepository.IsBank(bankName)) return Error.NotFound(400, $"Bank does not exist");
    
        var bank = bankRepository.GetByName(bankName);
        var approver = userAccountRepository.GetById(approverAccountId, bank!.Id);
        if (approver == null)
            return Error.NotFound(400, $"User account with id {approverAccountId} does not exist");

        if (!RolePermissionValidator.CanApproveLoan(approver.UserRole))
            return Error.AccessForbidden(403, "Not enough rights to approve loan");

        var loanRequest = loanRequestRepository.GetById(loanRequestId, bank.Id);
        if (loanRequest == null)
            return Error.NotFound(400, $"Loan request with id {loanRequestId} not found");

        var loanBankAccount = bankAccountRepository.GetById(loanRequest.BankAccountId, bank.Id);
        if (loanBankAccount == null)
            return Error.NotFound(400, $"Loan account for loan request {loanRequestId} not found");
        
        return Result.Success();
    }

    public Result<Guid> ApproveLoan(Guid loanRequestId, Guid approverAccountId, string bankName)
    {
        var resultValidation = ValidateInfo(loanRequestId, approverAccountId, bankName);
        if (!resultValidation.IsSuccess) return resultValidation.Error;

        var bank = bankRepository.GetByName(bankName);
        var loanRequest = loanRequestRepository.GetById(loanRequestId, bank.Id);
        var loanBankAccount = bankAccountRepository.GetById(loanRequest.BankAccountId, bank.Id);

        if (loanBankAccount.Status == BankAccountStatus.Pending)
            loanBankAccount.Status = BankAccountStatus.Active;
        else if (loanBankAccount.Status != BankAccountStatus.Active)
            return Error.AccessForbidden(403, "Invalid status of account to open the loan");

        Guid id = Guid.NewGuid();
        switch (loanRequest.LoanType)
        {
            case LoanType.Credit:
            {
                var credit = new Credit
                {
                    Id = id,
                    BankAccountId = loanRequest.BankAccountId,
                    BankId = loanRequest.BankId,
                    TotalAmount = loanRequest.TotalAmount,
                    TermMonths = loanRequest.TermMonths,
                    NextPaymentDate = DateTime.Now.AddMonths(1),
                    InterestRate = loanRequest.InterestRate
                };
                loanRepository.Add(credit);
                loanBankAccount.Balance += credit.TotalAmount;
                bankAccountRepository.Update(loanBankAccount);
                break;
            }

            case LoanType.Installment:
            {
                var installment = new Installment()
                {
                    Id = id,
                    BankAccountId = loanRequest.BankAccountId,
                    BankId = loanRequest.BankId,
                    TotalAmount = loanRequest.TotalAmount,
                    TermMonths = loanRequest.TermMonths,
                    NextPaymentDate = DateTime.Now.AddMonths(1),
                    InterestRate = loanRequest.InterestRate
                };
                loanRepository.Add(installment);
                break;
            }
            default:
            {
                return Error.Failure(400, "Invalid loan type");
            }
        }

        loanRequest.VerificationStatus = VerificationStatus.Approved;
        loanRequestRepository.Update(loanRequest);
        return id;
    }

    public Result RejectLoan(Guid loanRequestId, Guid approverAccountId, string bankName)
    {
        var resultValidation = ValidateInfo(loanRequestId, approverAccountId, bankName);
        if (!resultValidation.IsSuccess) return resultValidation.Error!;

        var bank = bankRepository.GetByName(bankName);
        var loanRequest = loanRequestRepository.GetById(loanRequestId, bank.Id);
        
        var bankAccount = bankAccountRepository.GetById(loanRequest.BankAccountId, bank.Id);
        if (bankAccount != null && bankAccount.Status == BankAccountStatus.Pending)
        {
            bankAccount.Status = BankAccountStatus.Blocked;
            bankAccountRepository.Delete(bankAccount);
        }

        loanRequest.VerificationStatus = VerificationStatus.Rejected;
        loanRequestRepository.Update(loanRequest);
        return Result.Success();
    }
}