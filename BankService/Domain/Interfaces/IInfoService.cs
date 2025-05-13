using BankService.Domain.Entities;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces;

public interface IInfoService
{
    public IEnumerable<string> GetEnterprises();
    public IEnumerable<string> GetBanks();
    public Result<IEnumerable<BankAccountDataDTO>> GetUserBankAccounts(Guid userAccountId, string bankName, BankAccountStatus bankAccountStatus);
    public Result<IEnumerable<BankAccountDataDTO>> GetEnterpriseBankAccounts(Guid userAccountId, string bankName, BankAccountStatus bankAccountStatus);
    public Result<IEnumerable<BankAccount>> GetOperatorBankAccounts(Guid userAccountId, string bankName, BankAccountStatus bankAccountStatus);
    public Result<IEnumerable<LoanDataDTO>> GetUserLoansInfo(Guid userAccountId, string bankName);
    public Result<IEnumerable<LoanRequest>> GetLoanRequests(Guid userAccountId, string bankName, VerificationStatus verificationStatus);
    public Result<IEnumerable<Loan>> GetUserLoans(Guid userAccountId, Guid requesterAccountId, string bankName);
    public Result<IEnumerable<UserAccount>> GetUserAccounts(Guid userAccountId, string bankName, VerificationStatus status);
    public Result<User> GetUserData(Guid userAccountId, Guid requesterAccountId, string bankName);
    public Result<IEnumerable<SalaryProjectRequestDataDto>> GetSalaryProjectRequests(Guid specialistUserAccountId, string bankName, VerificationStatus status);
    public Result<IEnumerable<SalaryProject>> GetBankSalaryProjects(Guid userAccountId, string bankName);
    public Result<IEnumerable<Transaction>> GetTransactions(Guid userAccountId, string bankName);
    
    public Result<TransferStatistics> GetTransferStatistics(Guid userAccountId, string bankName);
}