using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Interfaces.Services.TransactionServices;

namespace BankService.Application.UserInterationStrategies;

public class SpecialistMenuStrategy(
    IUserContext userContext,
    ISalaryProjectApprovalService salaryProjectApprovalService,
    IBankAccountRegistrationService bankAccountRegistrationService,
    IInfoService infoService,
    ITransferService transferService) : BaseMenuStrategy
{
    public override void ShowMenu()
    {
        Console.WriteLine("==== Specialist Menu ====");
        Console.WriteLine("1. Salary project operations");
        Console.WriteLine("2. Enterprise account operations");
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
    
    private Guid GetAccountId(int type)
    {
        while (true)
        {
            Console.WriteLine("Available actions:");
            Console.WriteLine("Guid. Enter valid ID");
            Console.WriteLine("1. Get {status} projects");
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

    private void SalaryProjectOperationsMenu()
    {
        Console.WriteLine("==== Salary project operations ====");
        Console.WriteLine("1. Approve employee's salary project request");
        Console.WriteLine("2. Reject employee's salary project request");
        Console.WriteLine("3. Form salary project request");
        Console.WriteLine("4. Get {status} employee's salary project request");
        Console.WriteLine("5. Pay salary project");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }

    private void EnterpriseAccountOperationsMenu()
    {
        Console.WriteLine("==== Enterprise account operations ====");
        Console.WriteLine("1. Create enterprise account");
        Console.WriteLine("2. Get list of enterprise accounts");
        Console.WriteLine("3. Top up enterprise account");
        Console.WriteLine("0. Exit");
        Console.WriteLine("-1. Logout");
    }
    
    private Guid GetRequestId(int type)
    {
        while (true)
        {
            Console.WriteLine("Available actions:");
            Console.WriteLine("Guid. Enter id");
            Console.WriteLine("1. See {status} requests");
            Console.WriteLine("0. Exit");
            Console.WriteLine("-1. Logout");
            var input = Console.ReadLine();
            if(input == "0")
                HandleAuthenticatedInput(0);
            if(input == "-1")
                HandleAuthenticatedInput(-1);
            if (input == "1")
                HandleInput(type);
            var result = Guid.TryParse(input, out Guid accountId);
            if (result == false)
                Console.WriteLine("Enter a valid guid");
            else
                return accountId;
        }
    }
    private void HandleAuthenticatedInput(int choice)
    {
        switch (choice)
        {
            case 1:
            {
                SalaryProjectOperationsMenu();

                var choiceSalaryProjectOperations = ReadInt();
                if (choiceSalaryProjectOperations <= 0)
                    HandleAuthenticatedInput(choiceSalaryProjectOperations);
                else
                    HandleAuthenticatedInput(choiceSalaryProjectOperations + 100);
                break;
            }
            case 101:
            {
                // approve request

                var requestId = GetRequestId(104);

                var resultApproval =
                    salaryProjectApprovalService.ApproveSalaryProjectRequest(userContext.UserAccountId.Value, requestId,
                        userContext.CurrentBank);

                if (!resultApproval.IsSuccess)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(resultApproval.Error!.Description);
                }
                else
                {
                    Console.WriteLine($"request {requestId} approved");
                }
                break;
            }
            case 102:
            {
                // reject request
                var requestId = GetRequestId(104);

                var resultRejecting =
                    salaryProjectApprovalService.RejectSalaryProjectRequest(userContext.UserAccountId.Value, requestId,
                        userContext.CurrentBank);

                if (!resultRejecting.IsSuccess)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(resultRejecting.Error!.Description);
                }
                else
                {
                    Console.WriteLine($"request {requestId} rejected");
                }
                break;
            }
            case 103:
            {
                // form salary request
                
                var requestId = GetString("project id", false);

                var resultForming = salaryProjectApprovalService.ApproveProject(userContext.UserAccountId.Value,
                    userContext.CurrentBank, requestId);

                if (!resultForming.IsSuccess)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(resultForming.Error!.Description);
                }
                else
                {
                    Console.WriteLine("Success!");
                    Console.WriteLine($"request {requestId} formed");
                }
                break;
            }
            case 104:
            {
                // get [status] requests

                var status = GetVerificationStatus();
                var infoResult = infoService.GetSalaryProjectRequests(userContext.UserAccountId.Value, userContext.CurrentBank, status);
                if (!infoResult.IsSuccess)
                {
                    Console.WriteLine("Error getting data");
                    Console.WriteLine($"\n{infoResult.Error!.Description}");
                }
                else
                {
                    var requests = infoResult.Value;
                    foreach (var request in requests)
                    {
                        Console.WriteLine($"- {request.Id}\n" +
                                          $"User data: {request.UserName} Passport: {request.PassportNumber}\n" +
                                          $"Project: {request.ProjectId} Salary: {request.Amount}");
                    }
                }
                break;
            }
            case 105:
            {
                // pay salary project

                var projectId = GetAccountId(106);
                var enterpriseAccountId = GetAccountId(202);
                var resultTransaction = transferService.PerformSalaryProjectTransaction(userContext.UserAccountId.Value,
                    userContext.CurrentBank, enterpriseAccountId, projectId);
                if (!resultTransaction.IsSuccess)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(resultTransaction.Error!.Description);
                }
                else
                {
                    Console.WriteLine("Success!");
                    Console.WriteLine($"request {projectId} payed");
                }
                break;
            }

            case 2:
            {
                // enterprise account operation
                EnterpriseAccountOperationsMenu();

                var choiceEnterpriseAccountOperations = ReadInt();
                if (choiceEnterpriseAccountOperations <= 0)
                    HandleAuthenticatedInput(choiceEnterpriseAccountOperations);
                else
                    HandleAuthenticatedInput(choiceEnterpriseAccountOperations + 200);
                break;
            }
            case 201:
            {
                // create enterprise account
                var resultCreationAccount = bankAccountRegistrationService.CreateBankAccount(userContext.CurrentBank, userContext.UserAccountId.Value, new AccountCreationDto
                {
                    Bank = userContext.CurrentBank,
                    Type = BankAccountType.Enterprise,
                    UserAccountId = userContext.UserAccountId.Value
                });

                if (!resultCreationAccount.IsSuccess)
                {
                    Console.WriteLine("Error");
                    Console.WriteLine(resultCreationAccount.Error!.Description);
                }
                else
                {
                    Console.WriteLine($"Account created successfully. Wait for approval");
                }
                
                break;
            }
            case 202:
            { 
                // list of enterprise accounts

                var status = GetBankAccountStatus();
                var resultAccounts = infoService.GetEnterpriseBankAccounts(userContext.UserAccountId.Value, userContext.CurrentBank, status);

                if (!resultAccounts.IsSuccess)
                {
                    Console.WriteLine("Error getting data");
                    Console.WriteLine(resultAccounts.Error!.Description);
                }
                else
                {
                    foreach (var account in resultAccounts.Value)
                    {
                        Console.WriteLine($"- {account.Id} Balance: {account.Balance}");
                    }
                }
                break;
            }
            case 203:
            {
                // top up
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
        }
    }
}