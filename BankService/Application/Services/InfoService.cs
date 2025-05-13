using BankService.Domain.Entities;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class InfoService(
    IEnterpriseRepository enterpriseRepository,
    IUserAccountRepository userAccountRepository,
    IBankAccountRepository bankAccountRepository,
    ILoanRepository loanRepository,
    ILoanRequestRepository loanRequestRepository,
    ISalaryProjectRequestRepository salaryProjectRequestRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository) : IInfoService
{
    public IEnumerable<string> GetEnterprises()
    {
        return enterpriseRepository.GetAllEnterprise();
    }

    public IEnumerable<string> GetBanks()
    {
        return enterpriseRepository.GetAllBanks();
    }

    private Result ValidateUser(Guid userAccountId, string bankName)
    {
        if (!enterpriseRepository.IsBank(bankName))
        {
            return Error.NotFound(400, "bank not found");
        }

        var bank = enterpriseRepository.GetByName(bankName);
        var userAccount = userAccountRepository.GetById(userAccountId, bank!.Id);
        if(userAccount == null)
            return Error.NotFound(400, "user account not found");
        if(userAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(400, "your account is not approved");

        return Result.Success();
    }

    private Result ValidateRequester(Guid userAccountId, string bankName)
    {
        var bank = enterpriseRepository.GetByName(bankName);
        if(bank == null)
            return Error.NotFound(400, "bank not found");
        if(!enterpriseRepository.IsBank(bankName))
            return Error.NotFound(400, "bank not found");
        var userAccount = userAccountRepository.GetById(userAccountId, bank.Id);
        if(userAccount == null)
            return Error.NotFound(400, "user not found");
        if (userAccount.Status != VerificationStatus.Approved || userAccount.UserRole < UserRole.Operator)
            return Error.AccessForbidden(400,  $"access forbidden for given account");
        return Result.Success();
    }

    private Result ValidateSpecialist(Guid userAccountId, string bankName)
    {
        var bank = enterpriseRepository.GetByName(bankName);
        if(bank == null)
            return Error.NotFound(400, "bank not found");
        if(!enterpriseRepository.IsBank(bankName))
            return Error.NotFound(400, "bank not found");
        var userAccount = userAccountRepository.GetById(userAccountId, bank.Id);
        if(userAccount == null)
            return Error.NotFound(400, "user account not found");
        if (userAccount.Status != VerificationStatus.Approved || userAccount.UserRole != UserRole.ExternalSpecialist)
            return Error.AccessForbidden(403,  $"access forbidden for given user account");
        return Result.Success();
    }

    public Result<IEnumerable<BankAccountDataDTO>> GetUserBankAccounts(Guid userAccountId, string bankName, BankAccountStatus bankAccountStatus)
    {
        var validationResult = ValidateUser(userAccountId, bankName);
        if(!validationResult.IsSuccess)
            return validationResult.Error!;

        var result = bankAccountRepository.GetAccounts(userAccountId, bankAccountStatus);
        return result.Select(r => new BankAccountDataDTO
        {
            Id = r.Id,
            AccountType = r.Type,
            Balance = r.Balance,
        }).ToList();
    }

    public Result<IEnumerable<BankAccountDataDTO>> GetEnterpriseBankAccounts(Guid userAccountId, string bankName, BankAccountStatus bankAccountStatus)
    {
        var validationResult = ValidateSpecialist(userAccountId, bankName);
        if(!validationResult.IsSuccess)
            return validationResult.Error!;
        
        var bank = enterpriseRepository.GetByName(bankName);
        var userAccount = userAccountRepository.GetById(
            userAccountId, bank!.Id);
        var result = bankAccountRepository.GetEnterpriseAccounts(userAccount!.EnterpriseId!.Value, bank.Id, bankAccountStatus);
        
        return result.Where(ba => ba.Type == BankAccountType.Enterprise).Select(r => new BankAccountDataDTO
        {
            Id = r.Id,
            AccountType = BankAccountType.Enterprise,
            Balance = r.Balance,
        }).ToList();
    }

    public Result<IEnumerable<BankAccount>> GetOperatorBankAccounts(Guid userAccountId, string bankName, BankAccountStatus bankAccountStatus)
    {
        var validationResult = ValidateUser(userAccountId, bankName);
        if(!validationResult.IsSuccess)
            return validationResult.Error!;

        var bank = enterpriseRepository.GetByName(bankName);
        var result = bankAccountRepository.GetAccountsForBank(bank.Id, bankAccountStatus);

        return result.ToList();
    }

    public Result<IEnumerable<LoanDataDTO>> GetUserLoansInfo(Guid userAccountId, string bankName)
    {
        
        var validationResult = ValidateUser(userAccountId, bankName);
        if(!validationResult.IsSuccess)
            return validationResult.Error!;
        
        var result = loanRepository.GetLoansByUserAccountId(userAccountId);
        
        return result.Select(l => new LoanDataDTO
        {
            LoanType = (l is Credit)? LoanType.Credit : LoanType.Installment,
            PaidAmount = l.PaidAmount,
            InterestRate = l.InterestRate,
            RemainingAmount = l.RemainingAmount,
            TermMonths = l.TermMonths,
            NextPaymentDate = l.NextPaymentDate
            
        }).ToList();
    }

    public Result<IEnumerable<LoanRequest>> GetLoanRequests(Guid userAccountId, string bankName, VerificationStatus verificationStatus)
    {
        var validationResult = ValidateUser(userAccountId, bankName);
        if(!validationResult.IsSuccess)
            return validationResult.Error!;

        var bank = enterpriseRepository.GetByName(bankName);
        var result = loanRequestRepository.GetLoanRequests(bank!.Id, verificationStatus);
        
        return result.ToList();
    }

    public Result<IEnumerable<Loan>> GetUserLoans(Guid userAccountId, Guid approverUserAccountId, string bankName)
    {
        var validationResult = ValidateUser(userAccountId, bankName);
        if(!validationResult.IsSuccess)
            return validationResult.Error!;
        var validationApproverResult = ValidateRequester(approverUserAccountId, bankName);
        if(!validationApproverResult.IsSuccess)
            return validationApproverResult.Error!;

        var result = loanRepository.GetLoansByUserAccountId(userAccountId);

        return result.ToList();
    }

    public Result<IEnumerable<UserAccount>> GetUserAccounts(Guid userAccountId, string bankName, VerificationStatus status)
    {
        var resultValidation = ValidateRequester(userAccountId, bankName);
        if (!resultValidation.IsSuccess)
            return resultValidation.Error!;
        
        var bank = enterpriseRepository.GetByName(bankName);
        var result = userAccountRepository.GetUserAccounts(bank!.Id, status);

        return result.ToList();
    }

    public Result<User> GetUserData(Guid userAccountId, Guid requesterAccountId, string bankName)
    {
        var resultValidation = ValidateRequester(requesterAccountId, bankName);
        if (!resultValidation.IsSuccess)
            return resultValidation.Error!;

        var bank = enterpriseRepository.GetByName(bankName);
        
        var userAccount = userAccountRepository.GetById(userAccountId, bank!.Id);
        if(userAccount == null)
            return Error.NotFound(400, "user not found");
        var user = userRepository.GetById(userAccount.UserId);
        if(user == null)
            return Error.NotFound(400, "user not found");
        
        return user;
    }

    public Result<IEnumerable<SalaryProjectRequestDataDto>> GetSalaryProjectRequests(Guid specialistUserAccountId, string bankName, VerificationStatus status)
    {
        var resultValidation = ValidateSpecialist(specialistUserAccountId, bankName);
        if (!resultValidation.IsSuccess)
            return resultValidation.Error!;

        var bank = enterpriseRepository.GetByName(bankName);
        var specialistAccount = userAccountRepository.GetById(specialistUserAccountId, bank!.Id);
        var result = salaryProjectRequestRepository.GetSalaryProjectRequests(specialistAccount!.EnterpriseId!.Value, status);
        return result.Select(x => new SalaryProjectRequestDataDto
        {
            Id = x.Id,
            UserName = x.EmployeeAccount.User.LastName + " " + x.EmployeeAccount.User.FirstName + " " + (x.EmployeeAccount.User.SecondName ?? ""),
            PassportNumber = x.EmployeeAccount.User.NationalPassportNumber,
            ProjectId = x.ProjectId,
            Amount = x.Salary
        }).ToList();
    }

    public Result<IEnumerable<SalaryProject>> GetBankSalaryProjects(Guid userAccountId, string bankName)
    {
        var resultValidation = ValidateRequester(userAccountId, bankName);
        if(!resultValidation.IsSuccess)
            return resultValidation.Error!;
        
        var bank = enterpriseRepository.GetByName(bankName);
        var userAccount = userAccountRepository.GetById(userAccountId, bank!.Id);
        var result = salaryProjectRepository.GetBankProjects(bank!.Id);
        return result.ToList();
    }

    public Result<IEnumerable<Transaction>> GetTransactions(Guid userAccountId, string bankName)
    {
        var resultValidation = ValidateRequester(userAccountId, bankName);
        if(!resultValidation.IsSuccess)
            return resultValidation.Error!;
        
        var bank = enterpriseRepository.GetByName(bankName);
        var transactions = transactionRepository.GetTransactions(bank!.Id);
        
        return transactions.ToList();
    }

    public Result<TransferStatistics> GetTransferStatistics(Guid userAccountId, string bankName)
    {
        var resultValidation = ValidateRequester(userAccountId, bankName);
        if(!resultValidation.IsSuccess)
            return resultValidation.Error!;
        
        var bank = enterpriseRepository.GetByName(bankName);
        var transactionsResult = transactionRepository.GetTransactions(bank!.Id);

        var transactions = transactionsResult.ToList();
        var statistic = new TransferStatistics
        {
            SalaryProjects = transactions.Count(x => x.Type == TransactionType.Salary),
            SalaryProjectsAmount = transactions.Where(x => x.Type == TransactionType.Salary).Sum(x => x.Amount),
            
            TotalTransaction = transactions.Count(),
            TotalTransactionAmount = transactions.Sum(x => x.Amount),
            
            TotalTransfers = transactions.Count(x => x.Type == TransactionType.Transfer || x.Type == TransactionType.Salary),
            TotalTransfersAmount = transactions.Where(x => x.Type == TransactionType.Transfer || x.Type == TransactionType.Salary).Sum(x => x.Amount),
            
            TransfersInsideBank = transactions
                .Count(x => (x.SenderBankAccount != null && x.SenderBankAccount.BankId == bank.Id) 
                                                          && (x.ReceiverBankAccount != null && x.ReceiverBankAccount.BankId == bank.Id)),
            TransfersInsideBankAmount = transactions
                .Where((x => (x.SenderBankAccount != null && x.SenderBankAccount.BankId == bank.Id) 
                                                                 && (x.ReceiverBankAccount != null && x.ReceiverBankAccount.BankId == bank.Id)))
                .Sum(x => x.Amount),

            TransfersOutsideBank = transactions
                .Count(x => (x.SenderBankAccount != null && x.SenderBankAccount.BankId != bank.Id) 
                            || (x.ReceiverBankAccount != null && x.ReceiverBankAccount.BankId != bank.Id)),
            TransfersOutsideBankAmount = transactions
                .Where(x => (x.SenderBankAccount != null && x.SenderBankAccount.BankId != bank.Id) 
                            || (x.ReceiverBankAccount != null && x.ReceiverBankAccount.BankId != bank.Id))
                .Sum(x => x.Amount)
        };
        return statistic;
    }
}