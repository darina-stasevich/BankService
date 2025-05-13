using System.Security.Cryptography;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.IValidators;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Interfaces.Services.TransactionServices;
using BankService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;

namespace BankService.Application.UserInterationStrategies;

public class ClientMenuStrategy(
    IUserContext userContext,
    IBankAccountRegistrationService bankAccountRegistrationService,
    IBankAccountApprovalService bankAccountApprovalService,
    IInfoService infoService,
    ILoanRequestDtoValidator loanRequestDtoValidator,
    ILoanRegistrationService loanRegistrationService,
    ISalaryProjectRegistrationService salaryProjectRegistrationService,
    ITransferService transferService) : BaseMenuStrategy
{
    
    private string? _enterpriseInfo = null;
    public override void ShowMenu()
    {
        Console.WriteLine("==== Client Menu ====");
        Console.WriteLine("Available actions:");
        Console.WriteLine("1. Operations with bank account");
        Console.WriteLine("2. Operations with loans");
        Console.WriteLine("3. Operations with salary project");
        Console.WriteLine("4. Transfers");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }
    public override void HandleInput(int choice)
    {
        if (!userContext.IsAuthenticated)
        {
            Console.WriteLine("\nSession expired. Please log in.");
            userContext.Clear();
            return;
        }

        HandleAuthenticatedInput(choice);
    }
    private void BankAccountOperationMenu()
    {
        Console.WriteLine("==== Bank Account operations ====");
        Console.WriteLine("1. Send request for opening bank account");
        Console.WriteLine("2. Send request for freezing bank account");
        Console.WriteLine("3. Send request for blocking bank account");
        Console.WriteLine("4. Get list of my active bank accounts");
        Console.WriteLine("5. Get list of my pending requests");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }
    private void LoanOperationMenu()
    {
        Console.WriteLine("==== Loan operations ====");
        Console.WriteLine("1. Send request for opening credit");
        Console.WriteLine("2. Send request for opening installment");
        Console.WriteLine("3. View current loans");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }
    private void TransfersMenu()
    {
        Console.WriteLine("==== Transfer operations ====");
        Console.WriteLine("1. Transfer money");
        Console.WriteLine("2. Withdraw money");
        Console.WriteLine("3. Top up money");
        Console.WriteLine("-1. Logout");
        Console.WriteLine("0. Exit");
    }
    private BankAccountType GetAccountType()
    {
        Console.WriteLine("Enter type of account to continue");
        Console.WriteLine("C - current account");
        Console.WriteLine("D - deposit account");
        Console.WriteLine("S - salary project");
        Console.WriteLine("Q - quit");
        while (true)
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "C": return BankAccountType.Current;
                case "D": return BankAccountType.Deposit;
                case "S": return BankAccountType.Salary;
                case "Q":
                {
                    HandleAuthenticatedInput(0);
                    break;
                }
                default:
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
                }
            }
        }
    }
    private void GetEnterprise()
    {
        _enterpriseInfo = null;
        
        while (_enterpriseInfo == null)
        {
            Console.WriteLine("Enter the name of the enterprise");
            Console.WriteLine("Available actions:");
            Console.WriteLine("1. Enter the name of the enterprise");
            Console.WriteLine("2. Get list of available enterprises");
            Console.WriteLine("0. Exit");
            Console.WriteLine("-1. Logout");
            var choice = ReadInt();
            if(choice <= 0)
                HandleAuthenticatedInput(choice);
            else
                HandleAuthenticatedInput(110 + choice);
        }
    }
    private DepositAccountOptionsDto GetDepositOptions()
    {
        Console.WriteLine("Enter true if you want early withdrawal, false otherwise");

        bool isAllowed;
        while (bool.TryParse(Console.ReadLine(), out isAllowed) == false)
            Console.WriteLine("Invalid input. Please try again.");

        Console.WriteLine("Enter interest rate");
        decimal interestRate = 0;
        while (decimal.TryParse(Console.ReadLine(), out interestRate) == false || interestRate <= 0)
            Console.WriteLine("Invalid input. Please try again.");

        Console.WriteLine("Enter maturity date in format dd.mm.yyyy");
        DateTime maturityDate;
        while (DateTime.TryParse(Console.ReadLine(), out maturityDate) == false)
            Console.WriteLine("Invalid input. Please try again.");
        
        return new DepositAccountOptionsDto
            { IsEarlyWithdrawalAllowed = isAllowed, InterestRate = interestRate, MaturityDate = maturityDate };
    }
    private Guid GetAccountId(bool canBeEmpty = false)
    {
        while (true)
        {
            Console.WriteLine("Available actions:");
            Console.WriteLine("Guid. Enter account ID");
            Console.WriteLine("1. See active bank accounts");
            Console.WriteLine("0. Exit");
            Console.WriteLine("-1. Logout");
            var input = Console.ReadLine();
            if(input == "0")
                HandleAuthenticatedInput(0);
            if(input == "-1")
                HandleAuthenticatedInput(-1);
            if (input == "1")
                HandleInput(104);
            if(input == "" && canBeEmpty)
                return Guid.Empty;
            var result = Guid.TryParse(input, out Guid accountId);
            if (result == false)
                Console.WriteLine("Enter a valid guid");
            else
                return accountId;
        }
    }
    private LoanRequestDTO GetLoanRequestDto(LoanType type)
    {
        Console.WriteLine("Enter bank account id to tie loan");
        var id = GetAccountId(true);
        Console.WriteLine("Enter term months");
        int termMonths = ReadInt();
        Console.WriteLine("Enter total amount");
        decimal totalAmount = ReadDecimal();
        Console.WriteLine("Enter interest rate");
        decimal interestRate = ReadDecimal();
        return new LoanRequestDTO
        {
            BankAccountId = id != Guid.Empty?id:null,
            TermMonths = termMonths,
            TotalAmount = totalAmount,
            InterestRate = interestRate,
            LoanType = type
        };
    }
    private void HandleAuthenticatedInput(int choice)
    {
        switch (choice)
        {
            case 1:
            {
                // operations with bank account

                BankAccountOperationMenu();
                int choiceBankAccountOperation = ReadInt();
                if (choiceBankAccountOperation <= 0)
                    HandleAuthenticatedInput(choiceBankAccountOperation);
                else
                    HandleAuthenticatedInput(choiceBankAccountOperation + 100);
                break;
            }
            case 101:
            {
                // open bank account

                if (!userContext.UserAccountId.IsSuccess)
                {
                    Console.WriteLine("Access error. Please log in.");
                    userContext.Clear();
                }

                var accountType = GetAccountType();
                var accountCreationDto = new AccountCreationDto
                {
                    Type = accountType
                };
                if (accountType == BankAccountType.Salary)
                {
                    GetEnterprise();
                    accountCreationDto.Enterprise = _enterpriseInfo;
                }

                if (accountType == BankAccountType.Deposit)
                {
                    accountCreationDto.DepositAccountOptionsDto = GetDepositOptions();
                }

                var resultCreateAccount =
                    bankAccountRegistrationService.CreateBankAccount(userContext.CurrentBank,
                        userContext.UserAccountId.Value, accountCreationDto);

                if (resultCreateAccount.IsSuccess)
                {
                    Console.WriteLine("Success! Your request is pending");
                }
                else
                {
                    Console.WriteLine("Error! Your request is invalid");
                    Console.WriteLine(resultCreateAccount.Error.Description);
                }
                // TODO logs
                break;
            }
            case 111:
            {
                var input = GetString("name of enterprise", false);
                _enterpriseInfo = input;
                
                break;
            }
            case 112:
            {
                var enterprises = infoService.GetEnterprises().ToList();
                foreach (var enterprise in enterprises)
                    Console.WriteLine(enterprise);
                if (enterprises.Count == 0)
                    Console.WriteLine();
                break;
            }
            case 102:
            {
                // freeze bank account
                var id = GetAccountId();
                var allAccounts = infoService.GetUserBankAccounts(userContext.UserAccountId.Value, userContext.CurrentBank,
                    BankAccountStatus.Freezed);
                if (allAccounts.IsSuccess == false){
                    Console.WriteLine("Error. Try again.");
                    break;
                }
                var result = allAccounts.Value.Any(x => x.Id == id);
                if (result == null)
                {
                    Console.WriteLine("Error. Incorrect id");
                }
                else
                {
                    var resultFreeze = bankAccountApprovalService.ChangeStatusBankAccount(id,
                        userContext.UserAccountId.Value,
                        userContext.CurrentBank, BankAccountStatus.Freezed
                    );
                    if (resultFreeze.IsSuccess)
                        Console.WriteLine("Success! Account will be unfrozen after 24 hours.");
                    else
                    {
                        Console.WriteLine("Error! Freezing failed");
                    }
                    // TODO logs
                }
                break;
            }
            case 103:
            {
                // block bank account
                var id = GetAccountId();
                var allAccounts = infoService.GetUserBankAccounts(userContext.UserAccountId.Value, userContext.CurrentBank,
                    BankAccountStatus.Blocked);
                if (allAccounts.IsSuccess == false){
                    Console.WriteLine("Error. Try again.");
                    break;
                }
                var result = allAccounts.Value.Any(x => x.Id == id);
                if (result == null)
                {
                    Console.WriteLine("Error. Incorrect id");
                }
                else
                {
                    var resultFreeze = bankAccountApprovalService.ChangeStatusBankAccount(id,
                        userContext.UserAccountId.Value,
                        userContext.CurrentBank, BankAccountStatus.Freezed
                    );
                    if (resultFreeze.IsSuccess)
                        Console.WriteLine("Success! Account is blocked.");
                    else
                    {
                        Console.WriteLine("Error! Blocking failed");
                        Console.WriteLine(resultFreeze.Error.Description);
                    }
                    // TODO logs
                }
                break;
            }
            case 104:
            {
                // active bank accounts
                
                var resultList = infoService.GetUserBankAccounts(userContext.UserAccountId.Value, userContext.CurrentBank, BankAccountStatus.Active);
                if(!resultList.IsSuccess)
                    Console.WriteLine("Error. try again");
                else
                {
                    var list = resultList.Value.ToList();
                    foreach (var account in list)
                    {
                        Console.WriteLine($"- {account.Id} : {account.AccountType.ToString()}. Balance = {account.Balance}");
                    }

                    if (list.Count == 0)
                        Console.WriteLine();
                }

                break;
            }
            case 105:
            {
                // pending bank accounts
                var resultList = infoService.GetUserBankAccounts(userContext.UserAccountId.Value, userContext.CurrentBank, BankAccountStatus.Pending);
                if(!resultList.IsSuccess)
                    Console.WriteLine("Error. try again");
                else
                {
                    var list = resultList.Value.ToList();
                    foreach (var account in list)
                    {
                        Console.WriteLine($"- {account.Id} : {account.AccountType.ToString()}. Balance = {account.Balance}");
                    }

                    if (list.Count == 0)
                        Console.WriteLine();
                }
                break;
            }
            case 2:
            {
                // operations with loans
                
                LoanOperationMenu();
                int choiceLoanOperation = ReadInt();
                if(choiceLoanOperation <= 0)
                    HandleAuthenticatedInput(choiceLoanOperation);
                else
                    HandleAuthenticatedInput(choiceLoanOperation + 200);
                break;
            }
            case 201:
            {
                var dto = GetLoanRequestDto(LoanType.Credit);
                var resultValidation = loanRequestDtoValidator.Validate(dto);
                if (!resultValidation.IsValid)
                {
                    Console.WriteLine("Error. You entered invalid data.");
                    foreach(var error in resultValidation.Errors)
                        Console.WriteLine(error.ErrorMessage);
                }
                else
                {
                    Console.WriteLine("Data validated successfully");
                    var result = loanRegistrationService.RegisterLoan(userContext.UserAccountId.Value, userContext.CurrentBank, dto);
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine("Error registering loan");
                    }
                    else
                    {
                        Console.WriteLine("Success! Your credit request is pending");
                        Console.WriteLine($"number of request {result.Value}");
                    }
                }
                break;
            }
            case 202:
            {
                var dto = GetLoanRequestDto(LoanType.Installment);
                var resultValidation = loanRequestDtoValidator.Validate(dto);
                if (!resultValidation.IsValid)
                {
                    Console.WriteLine("Error. You entered invalid data.");
                    Console.WriteLine(resultValidation.Errors.ToString());
                }
                else
                {
                    Console.WriteLine("Data validated successfully");
                    var result = loanRegistrationService.RegisterLoan(userContext.UserAccountId.Value, userContext.CurrentBank, dto);
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine("Error registering loan");
                        Console.WriteLine(result.Error.Description);
                    }
                    else
                    {
                        Console.WriteLine("Success! Your installment request is pending");
                        Console.WriteLine($"number of request {result.Value}");
                    }
                }
                break;
            }
            case 203:
            {
                // get current loans
                var result = infoService.GetUserLoansInfo(userContext.UserAccountId.Value, userContext.CurrentBank);
                if(!result.IsSuccess)
                    Console.WriteLine("Error. try again");
                else
                {
                    var list = result.Value.ToList();
                    foreach (var loan in list)
                    {
                        Console.WriteLine($"{loan.LoanType}: rate - {loan.InterestRate}%. Term - {loan.TermMonths} Paid:{loan.PaidAmount} Remain: {loan.RemainingAmount} Next payment: {loan.NextPaymentDate}");
                    }
                }
                break;
            }
            case 3:
            {
                // salary project registration
                
                Console.WriteLine("Enter salary account id. Be sure you chose correct enterprise");
                var bankAccountId = GetAccountId();
                var projectId = GetString("project id", false);
                Console.WriteLine("Enter salary amount");
                var amount = ReadDecimal();
                var dto = new SalaryProjectRequestDTO
                {
                    SalaryAccountId = bankAccountId,
                    Amount = amount,
                    ProjectId = projectId!,
                    BankName = userContext.CurrentBank!
                };
                
                var requestResult =
                    salaryProjectRegistrationService.CreateSalaryProjectRequest(userContext.UserAccountId.Value, dto);
                if (!requestResult.IsSuccess)
                {
                    Console.WriteLine("Error registration salary request. Try again.");
                    Console.WriteLine(requestResult.Error.Description);
                }
                else
                {
                    Console.WriteLine("Success! Salary project request is pending. Wait for approval");
                }
                break;
            }
            case 4:
            {
                // Transfer operations
                
                TransfersMenu();
                int choiceTransfers = ReadInt();
                if(choiceTransfers <= 0)
                    HandleAuthenticatedInput(choiceTransfers);
                else
                    HandleAuthenticatedInput(choiceTransfers + 400);
                break;
            }
            case 401:
            {
                Console.WriteLine("Enter bank account id you want to transfer from");
                var senderBankAccountId = GetAccountId();
                var nameBank = GetString("bank name receiver account is tied to.", false);
                Console.WriteLine("Enter bank account id you want to transfer to");
                var receiverBankAccountId = GetGuid();
                Console.WriteLine("Enter amount you want to transfer");
                var amount = ReadDecimal();

                var request = new TransferRequest
                {
                    SenderAccountId = senderBankAccountId,
                    ReceiverAccountId = receiverBankAccountId,
                    Amount = amount
                };
                var resultTransaction = transferService.PerformTransfer(userContext.UserAccountId.Value, request, userContext.CurrentBank, nameBank);
                if (!resultTransaction.IsSuccess)
                {
                    Console.WriteLine("Error. Performing transfer failed. Try again.");
                    Console.WriteLine(resultTransaction.Error.Description);
                }
                else
                {
                    Console.WriteLine("Success! Transfer successful");
                }

                break;
            }
            case 402:
            {
                // withdraw
                Console.WriteLine("Enter bank account id you want to withdraw money from.");
                var senderBankAccountId = GetAccountId();
                Console.WriteLine("Enter amount you want to withdraw");
                var amount = ReadDecimal();

                if (!userContext.UserAccountId.IsSuccess || userContext.CurrentBank == null)
                {
                    Console.WriteLine("Error authorization. Try again.");
                }
                else
                {
                    var request = new TransferRequest
                    {
                        SenderAccountId = senderBankAccountId,
                        Amount = amount
                    };
                    var resultWithdraw = transferService.PerformWithdraw(userContext.UserAccountId.Value, request,
                        userContext.CurrentBank!);

                    if (!resultWithdraw.IsSuccess)
                    {
                        Console.WriteLine("Error. Performing top up failed. Try again.");
                        Console.WriteLine(resultWithdraw.Error!.Description);
                    }
                    else
                    {
                        Console.WriteLine("Success! Top up successful");
                    }
                }

                break;
            }
            case 403:
            {
                // top up
                Console.WriteLine("Enter bank account id you want to put money on.");
                var receiverAccountId = GetAccountId();
                Console.WriteLine("Enter amount you want to put in");
                var amount = ReadDecimal();

                if (!userContext.UserAccountId.IsSuccess || userContext.CurrentBank == null)
                {
                    Console.WriteLine("Error authorization. Try again.");
                }
                else
                {
                    var request = new TransferRequest
                    {
                        ReceiverAccountId = receiverAccountId,
                        Amount = amount
                    };
                    var resultTopUp = transferService.TopUp(userContext.UserAccountId.Value, request,
                        userContext.CurrentBank!);

                    if (!resultTopUp.IsSuccess)
                    {
                        Console.WriteLine("Error. Performing top up failed. Try again.");
                        Console.WriteLine(resultTopUp.Error!.Description);
                    }
                    else
                    {
                        Console.WriteLine("Success! Top up successful");
                    }
                }
                break;
            }
            case 0:
            {
                Environment.Exit(0);
                break;
            }
            case -1:
            {
                userContext.Clear();
                break;
            }
            default:
            {
                Console.WriteLine("Invalid choice");
                break;
            }
        }
    }
}