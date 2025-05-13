using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Interfaces.Services.TransactionServices;

namespace BankService.Application.UserInterationStrategies;

public class AdministratorMenuStrategy(
    IUserContext userContext,
    IInfoService infoService,
    IUserAccountApprovalService userAccountApprovalService,
    IBankAccountApprovalService bankAccountApprovalService,
    ILoanApprovalService loanApprovalService,
    ISalaryProjectApprovalService salaryProjectApprovalService,
    ITransferService transferService
    ) : BaseMenuStrategy
{
    public override void ShowMenu()
    {
        Console.WriteLine("==== Administrator Menu ====");
        Console.WriteLine("Available actions:");
        Console.WriteLine("1. User account operations");
        Console.WriteLine("2. Bank account operations");
        Console.WriteLine("3. Salary project operations");
        Console.WriteLine("4. Loan operations");
        Console.WriteLine("5. Get statistics");
        Console.WriteLine("6. Revert operations");
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

    private void RevertTransactionMenu()
    {
        Console.WriteLine("==== Revert Transaction ====");
        Console.WriteLine("1. Revert transaction");
        Console.WriteLine("2. Get transactions");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }

    private void UserAccountOperationsMenu()
    {
        Console.WriteLine("==== User Account Operations ====");
        Console.WriteLine("1. Approve account");
        Console.WriteLine("2. Reject account");
        Console.WriteLine("3. Get {status} accounts");
        Console.WriteLine("4. Get user information");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }

    private void BankAccountOperationsMenu()
    {
        Console.WriteLine("==== Bank Account Operations ====");
        Console.WriteLine("1. Change account status");
        Console.WriteLine("2. Get {status} accounts");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }

    private void LoanOperationsMenu()
    {
        Console.WriteLine("==== Loan Operations ====");
        Console.WriteLine("1. Approve loan request");
        Console.WriteLine("2. Reject loan request");
        Console.WriteLine("3. Get {status} loan requests");
        Console.WriteLine("4. Get loans for user account");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }
    
    private void SalaryProjectOperationsMenu()
    {
        Console.WriteLine("==== Salary Project Operations ====");
        Console.WriteLine("1. approve salary project");
        Console.WriteLine("2. Get list of projects");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }
    private Guid GetAccountId(int type)
    {
        while (true)
        {
            Console.WriteLine("Available actions:");
            Console.WriteLine("Guid. Enter account ID");
            Console.WriteLine("1. Get {status} accounts/requests");
            Console.WriteLine("0. Exit");
            Console.WriteLine("-1. Logout");
            var input = Console.ReadLine();
            if (input == "0")
                HandleAuthenticatedInput(0);
            if (input == "-1")
                HandleAuthenticatedInput(-1);
            if (input == "1")
                HandleInput(type);
            var result = Guid.TryParse(input, out var accountId);
            if (result == false)
                Console.WriteLine("Enter a valid guid");
            else
                return accountId;
        }
    }

    private VerificationStatus GetVerificationStatus()
    {
        while (true)
        {
            var status = GetString("verification status", false)!;
            var parseResult = Enum.TryParse(status, out VerificationStatus receivedVerificationStatus);
            if (parseResult) return receivedVerificationStatus;
        }
    }

    private BankAccountStatus GetBankAccountStatus()
    {
        while (true)
        {
            var status = GetString("bank account status", false)!;
            var parseResult = Enum.TryParse(status, out BankAccountStatus receivedVerificationStatus);
            if (parseResult) return receivedVerificationStatus;
        }
    }

    private void HandleAuthenticatedInput(int choice)
    {
        switch (choice)
        {
            case 1:
            {
                // operations with user account

                UserAccountOperationsMenu();

                var choiceUserAccountOperations = ReadInt();
                if (choiceUserAccountOperations <= 0)
                    HandleAuthenticatedInput(choiceUserAccountOperations);
                else
                    HandleAuthenticatedInput(choiceUserAccountOperations + 100);
                break;
            }
            case 101:
            {
                // approve account
                var accountId = GetAccountId(103);
                var resultApproval = userAccountApprovalService.ApproveAccount(accountId,
                    userContext.UserAccountId.Value,
                    userContext.CurrentBank);
                if (resultApproval.IsSuccess)
                {
                    Console.WriteLine("Account approved successfully");
                }
                else
                {
                    Console.WriteLine("Account approval failed");
                    Console.WriteLine(resultApproval.Error!.Description);
                }

                break;
            }
            case 102:
            {
                // reject account
                var accountId = GetAccountId(103);
                var resultRejection = userAccountApprovalService.RejectAccount(accountId,
                    userContext.UserAccountId.Value,
                    userContext.CurrentBank);
                if (resultRejection.IsSuccess)
                {
                    Console.WriteLine("Account approved successfully");
                }
                else
                {
                    Console.WriteLine("Account approval failed");
                    Console.WriteLine(resultRejection.Error!.Description);
                }

                break;
            }
            case 103:
            {
                // list of {status} accounts

                var resultListUsers = infoService.GetUserAccounts(userContext.UserAccountId.Value,
                    userContext.CurrentBank, GetVerificationStatus());

                if (!resultListUsers.IsSuccess)
                    Console.WriteLine("Error getting data. Try again");
                else
                    foreach (var userAccount in resultListUsers.Value)
                        Console.WriteLine($"- {userAccount.Id}\n" +
                                          $"EnterpriseID: {userAccount.EnterpriseId}\n " +
                                          $"User role: {userAccount.UserRole}. Login {userAccount.Login}");

                break;
            }
            case 104:
            {
                // data of user account

                var userAccountId = GetAccountId(103);
                var result = infoService.GetUserData(userAccountId, userContext.UserAccountId.Value,
                    userContext.CurrentBank);
                if (!result.IsSuccess)
                {
                    Console.WriteLine("Error getting data. Try again");
                    Console.WriteLine(result.Error!.Description);
                }
                else
                {
                    var user = result.Value;
                    Console.WriteLine($"Id: {user.Id} \n" +
                                      $"{user.LastName} {user.FirstName} {user.SecondName} \n" +
                                      $"{user.Email}\n" +
                                      $"{user.PhoneNumber}\n" +
                                      $"IsResident: {user.IsResident} Passport number: {(user.IsResident ? user.NationalPassportNumber : user.ForeignPassportNumber)}" +
                                      $" Passport ID: {(user.IsResident ? user.NationalPassportID : user.ForeignPassportID)}\n" +
                                      $"Status: {user.Status}");
                }

                break;
            }
            case 2:
            {
                // operations with bank account

                BankAccountOperationsMenu();

                var choiceBankAccountOperations = ReadInt();
                if (choiceBankAccountOperations <= 0)
                    HandleAuthenticatedInput(choiceBankAccountOperations);
                else
                    HandleAuthenticatedInput(choiceBankAccountOperations + 200);
                break;
            }
            case 201:
            {
                // change bank account status

                var accountId = GetAccountId(202);
                var status = GetBankAccountStatus();
                var resultChangeStatus = bankAccountApprovalService.ChangeStatusBankAccount(accountId,
                    userContext.UserAccountId.Value, userContext.CurrentBank, status);
                if (resultChangeStatus.IsSuccess)
                {
                    Console.WriteLine("Account status changed successfully");
                }
                else
                {
                    Console.WriteLine("Account status changing failed");
                    Console.WriteLine(resultChangeStatus.Error!.Description);
                }

                break;
            }
            case 202:
            {
                // get list of bank accounts
                var status = GetBankAccountStatus();
                var resultGettingAccounts =
                    infoService.GetOperatorBankAccounts(userContext.UserAccountId.Value, userContext.CurrentBank, status);

                if (!resultGettingAccounts.IsSuccess)
                {
                    Console.WriteLine("Failed to get bank accounts");
                    Console.WriteLine(resultGettingAccounts.Error!.Description);
                }
                else
                {
                    foreach (var account in resultGettingAccounts.Value)
                    {
                        Console.WriteLine($"- {account.Id} Type: {account.Type.ToString()}\n" +
                                          $"UserID : {(account.UserAccountId != null?account.UserAccountId:"")} EnterpriseID : {(account.EnterpriseId != null?account.EnterpriseId:"")}");
                        Console.WriteLine($"Status: {account.Status.ToString()} Balance: {account.Balance}");
                        Console.WriteLine($"{(account.FrozenTill != null? account.FrozenTill.ToString() : "")}{(account.BlockedDate != null ? account.BlockedDate.ToString() : "")}");
                        if (account is DepositAccount depositAccount)
                        {
                            Console.WriteLine($"Interest rate: {depositAccount.InterestRate} Maturity date: {depositAccount.MaturityDate}");
                        }
                    }
                }
                break;
            }
            case 3:
            {
                // operations with salary project
                
                SalaryProjectOperationsMenu();
                var choiceSalaryProjectOperations = ReadInt();
                if(choiceSalaryProjectOperations <= 0)
                    HandleAuthenticatedInput(choiceSalaryProjectOperations);
                else
                    HandleAuthenticatedInput(300 + choiceSalaryProjectOperations);
                break;
            }
            case 301:
            {
                // approve project

                var projectId = GetAccountId(302);
                var resultApproval = salaryProjectApprovalService.ApproveProject(userContext.UserAccountId.Value, userContext.CurrentBank, projectId.ToString());
                if (!resultApproval.IsSuccess)
                {
                    Console.WriteLine("Error approval");
                    Console.WriteLine(resultApproval.Error!.Description);
                }
                else
                {
                    Console.WriteLine("Project approved successfully"); 
                }
                break;
            }
            case 302:
            {
                // get list of projects
                var resultProject = infoService.GetBankSalaryProjects(userContext.UserAccountId.Value, userContext.CurrentBank);

                if (!resultProject.IsSuccess)
                {
                    Console.WriteLine("Failed to get bank salary projects");
                    Console.WriteLine(resultProject.Error!.Description);
                }
                else
                {
                    foreach (var project in resultProject.Value)
                    {
                        Console.WriteLine($"- {project.Id} EnterpriseId: {project.EnterpriseId}\n" +
                                          $"Amount: {project.Amount} Status: {project.Status} Request date: {project.SendRequestDate}");
                    }
                }
                break;
            }
            case 4:
            {
                // operations with loans
                
                LoanOperationsMenu();
                
                var choiceLoanOperations = ReadInt();
                if (choiceLoanOperations <= 0)
                    HandleAuthenticatedInput(choiceLoanOperations);
                else
                    HandleAuthenticatedInput(choiceLoanOperations + 400);
                break;
            }
            case 401:
            {
                var requestId = GetAccountId(403);
                var resultApproval = loanApprovalService.ApproveLoan(requestId,
                    userContext.UserAccountId.Value,
                    userContext.CurrentBank);
                if (resultApproval.IsSuccess)
                {
                    Console.WriteLine($"loan request was approved successfully. Id of loan is {resultApproval.Value}");
                }
                else
                {
                    Console.WriteLine("loan request approval failed");
                    Console.WriteLine(resultApproval.Error!.Description);
                }
                break;
            }
            case 402:
            {
                var requestId = GetAccountId(403);
                var resultRejection = loanApprovalService.RejectLoan(requestId,
                    userContext.UserAccountId.Value,
                    userContext.CurrentBank);
                if (resultRejection.IsSuccess)
                {
                    Console.WriteLine("loan request was rejected successfully.");
                }
                else
                {
                    Console.WriteLine("loan request rejection failed");
                    Console.WriteLine(resultRejection.Error!.Description);
                }
                break;
            }
            case 403:
            {
                var status = GetVerificationStatus();
                var resultLoanRequests = infoService.GetLoanRequests(userContext.UserAccountId.Value, userContext.CurrentBank, status);
                if (!resultLoanRequests.IsSuccess)
                {
                    Console.WriteLine("Failed to get loan requests");
                    Console.WriteLine(resultLoanRequests.Error!.Description);
                }
                else
                {
                    foreach (var loanRequest in resultLoanRequests.Value)
                    {
                        Console.WriteLine($"- {loanRequest.Id} : {loanRequest.LoanType.ToString()}\n" +
                                          $"BankAccountID: {loanRequest.BankAccountId}\n" +
                                          $"Amount: {loanRequest.TotalAmount} Term Months: {loanRequest.TermMonths} Interest Rate: {loanRequest.InterestRate}");
                    }
                }
                break;
            }
            case 404:
            {
                var accountId = GetAccountId(103);
                var resultGettingInfo = infoService.GetUserLoans(accountId, userContext.UserAccountId.Value,
                    userContext.CurrentBank);
                if (!resultGettingInfo.IsSuccess)
                {
                    Console.WriteLine("Error getting data");
                    Console.WriteLine(resultGettingInfo.Error!.Description);
                }
                else
                {
                    foreach (var loan in resultGettingInfo.Value)
                    {
                        Console.WriteLine($"- {loan.Id} - {(loan is Credit? "credit" : "loan")}\n" +
                                          $"bankAccountID: {loan.BankAccountId}\n" +
                                          $"Total: {loan.TotalAmount} TermMonths: {loan.TermMonths} InterestRate: {loan.InterestRate}\n" +
                                          $"Next payment date: {loan.NextPaymentDate}");
                    }
                }
                break;
            }
            case 5:
            {
                var statisticsResult = infoService.GetTransferStatistics(userContext.UserAccountId.Value, userContext.CurrentBank);
                if (!statisticsResult.IsSuccess)
                {
                    Console.WriteLine("Failed to get statistics");
                    Console.WriteLine(statisticsResult.Error!.Description);
                }
                else
                {
                    var statistics = statisticsResult.Value;
                    Console.WriteLine($"{statistics.ToString()}");
                }
                break;
            }
            case 6: 
            {
                RevertTransactionMenu();
                var choiceRevertTransaction = ReadInt();
                if(choiceRevertTransaction <= 0)
                    HandleAuthenticatedInput(choiceRevertTransaction);
                else
                    HandleAuthenticatedInput(600 + choiceRevertTransaction);
                break;
            }
            case 601:
            {
                // revert
                
                var transactionId = GetAccountId(602);
                
                var resultRevert = transferService.CancelTransaction(userContext.UserAccountId.Value, userContext.CurrentBank, transactionId);
                if (!resultRevert.IsSuccess)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(resultRevert.Error!.Description);
                }
                else
                {
                    Console.WriteLine("Reverted successfully");
                }
                
                break;
            }
            case 602:
            {
                // get list of transactions
                var resultInfo = infoService.GetTransactions(userContext.UserAccountId.Value, userContext.CurrentBank);
                if (!resultInfo.IsSuccess)
                {
                    Console.WriteLine("Failed to get transactions");
                    Console.WriteLine(resultInfo.Error!.Description);
                }
                else
                {
                    var transactions = resultInfo.Value;
                    foreach (var transaction in transactions)
                    {
                        Console.WriteLine($"- {transaction.Id} {transaction.Type.ToString()}\n" +
                                          $"Sender: {transaction.SenderAccountId} Receiver: {transaction.ReceiverAccountId} {(transaction.SalaryProjectId != null? $"ProjectId: {transaction.SalaryProjectId}":"")} Amount: {transaction.Amount} Status: {transaction.Status.ToString()}");
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
