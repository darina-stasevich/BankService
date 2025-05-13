using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class LoanRegistrationService(
    IUserAccountRepository userAccountRepository,
    IEnterpriseRepository bankRepository,
    IBankAccountRepository bankAccountRepository,
    ILoanRequestRepository loanRequestRepository,
    IAccountFactory accountFactory
) : ILoanRegistrationService
{
    public Result<Guid> RegisterLoan(Guid userAccountId, string bankName, LoanRequestDTO loanRequestDto)
    {
        
        if (!bankRepository.IsBank(bankName))
            return Error.NotFound(400, $"bank with id: {bankName} not found");

        var bank = bankRepository.GetByName(bankName);
        var userAccount = userAccountRepository.GetById(userAccountId, bank.Id);
        if (userAccountId == null)
            return Error.NotFound(400, $"user account with id: {userAccountId} not found");
        if(userAccount.UserRole != UserRole.Client)
            return Error.AccessForbidden(403, "You are not allowed to register this loan");
        BankAccount? bankAccount = null;
        if (loanRequestDto.BankAccountId == null)
        {
            // need to create new bank account
            AccountCreationDto accountCreationDto = new AccountCreationDto
            {
                UserAccountId = userAccount.Id,
                Bank = bankName,
                Type = BankAccountType.Current
            };
            var accountCreationResult = accountFactory.CreateAccount(accountCreationDto);
            if (!accountCreationResult.IsSuccess)
                return accountCreationResult.Error;
            bankAccount = accountCreationResult.Value;
            bankAccountRepository.Add(bankAccount);
        }
        else
        {
            // already has account to tie loan
            bankAccount = bankAccountRepository.GetById(loanRequestDto.BankAccountId.GetValueOrDefault(), bank.Id);

            if (bankAccount == null)
                return Error.NotFound(400, $"bank account with id: {bankAccount} not found");

            switch (loanRequestDto.LoanType)
            {
                case LoanType.Credit:
                {
                    if (bankAccount.CreditAllowed == false)
                        return Error.Failure(400,
                            $"bank account with id: {bankAccount} does not have credit allowed");

                    break;
                }
                case LoanType.Installment:
                {
                    if (bankAccount.InstallmentAllowed == false)
                        return Error.Failure(400,
                            $"bank account with id: {bankAccount} does not have installment allowed");
                    break;
                }
                default:
                {
                    return Error.NotFound(400, $"loans with type: {loanRequestDto.LoanType} not exists");
                }
            }

            if (bankAccount.Status != BankAccountStatus.Active)
                return Error.Failure(400, "only active bank accounts can have loans");
        }

        var request = new LoanRequest
        {
            BankId = bank.Id,
            BankAccountId = bankAccount.Id,
            LoanType = loanRequestDto.LoanType,
            TotalAmount = loanRequestDto.TotalAmount,
            TermMonths = loanRequestDto.TermMonths,
            InterestRate = loanRequestDto.InterestRate,
            VerificationStatus = VerificationStatus.Pending
        };

        loanRequestRepository.Add(request);
        return request.Id;
    }
}