using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using BankService.Application.Validators;
using BankService.Domain.Entities;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.IValidators;
using BankService.Domain.Interfaces.Services.TransactionServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class TransferService(IBankAccountRepository bankAccountRepository,
    IEnterpriseRepository bankRepository,
    ITransferAccountStatusValidator transferAccountStatusValidator,
    ITransferValidator transferValidator,
    IWithdrawValidator withdrawValidator,
    IWithdrawAccountValidator withdrawAccountValidator,
    IUserAccountRepository userAccountRepository,
    ITransactionRepository transactionRepository,
    ISalaryProjectRepository salaryProjectRepository,
    ISalaryProjectRequestRepository salaryProjectRequestRepository) : ITransferService
{
    public Transaction CreateTransaction(TransactionType type, Guid? senderId, Guid? receiverId, decimal amount)
    {
        return new Transaction
        {
            Type = type,
            Status = TransactionStatus.Performed,
            SenderAccountId = senderId,
            ReceiverAccountId = receiverId,
            Amount = amount,
            Date = DateTime.Now
        };
    }
    
    public Result PerformTransfer(Guid userAccountId, TransferRequest request, string senderBankName, string receiverBankName)
    {
        var result = transferValidator.Validate(request);
        if (!result.IsValid)
            return Error.Validation(400,
                string.Join(", ", result.Errors!.Select(e => e.ErrorMessage)));

        var senderBank = bankRepository.GetByName(senderBankName);
        if(senderBank == null)
            return Error.NotFound(400, "sender bank not found");
        if(!bankRepository.IsBank(senderBankName))
            return Error.NotFound(400, "sender bank not found");

        var senderUserAccount = userAccountRepository.GetById(userAccountId, senderBank.Id);
        if(senderUserAccount == null)
            return Error.AccessForbidden(403, "sender user account not found");
        if(senderUserAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "sender user account is not approved");
        var senderBankAccount = bankAccountRepository.GetById(request.SenderAccountId.Value, senderBank.Id);

        if (senderBankAccount == null)
            return Error.NotFound(400,
                $"sender account with id: {request.SenderAccountId} not found");
        if (senderBankAccount.UserAccountId != userAccountId)
            return Error.AccessForbidden(403,"invalid bank account id");
        
        var isBank = bankRepository.IsBank(receiverBankName);
        if(!isBank)
            return Error.Validation(400, "Enterprise is not a valid bank");
        var receiverBank = bankRepository.GetByName(receiverBankName);
        if (receiverBank == null)
            return Error.NotFound(400, "receiver bank not found");
        
        var receiverBankAccount = bankAccountRepository.GetById(request.ReceiverAccountId.Value, receiverBank.Id);

        if (receiverBankAccount == null)
            return Error.NotFound(400,
                $"receiver account with id : {request.ReceiverAccountId} not found");

        var tupleStatus =
            new Tuple<BankAccount, BankAccount>(senderBankAccount, receiverBankAccount);
        result = transferAccountStatusValidator.Validate(tupleStatus);
        
        if (!result.IsValid)
            return Error.Validation(400,
                string.Join(", ", result.Errors!.Select(e => e.ErrorMessage)));

        if (senderBankAccount.Balance < request.Amount)
            return Error.Failure(400, "There are not enough money to perform transfer.");

        senderBankAccount.Balance -= request.Amount;
        receiverBankAccount.Balance += request.Amount;
        bankAccountRepository.Update(senderBankAccount);
        bankAccountRepository.Update(receiverBankAccount);
        var transaction = CreateTransaction(TransactionType.Transfer, request.SenderAccountId, request.ReceiverAccountId.Value, request.Amount);
        transactionRepository.Add(transaction);
        return Result.Success();
    }
    
    public Result TopUp(Guid userAccountId, TransferRequest request, string bankName)
    {
        var bank = bankRepository.GetByName(bankName);
        if(bank == null)
            return Error.NotFound(400, "bank not found");
        if(!bankRepository.IsBank(bankName))
            return Error.NotFound(400, "bank not found");
        
        var receiverUserAccount = userAccountRepository.GetById(userAccountId, bank.Id);
        if(receiverUserAccount == null)
            return Error.AccessForbidden(403, "sender user account not found");
        if(receiverUserAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "sender user account is not approved");
        if (request.ReceiverAccountId == null)
        {
            return Error.NotFound(400, "receiver account not found");
        }
        var resultValidateRequest = withdrawValidator.Validate(request);
        if (!resultValidateRequest.IsValid)
        {
            return Error.Validation(400, string.Join(", ",
                resultValidateRequest.Errors.Select(x => x.ErrorMessage)));
        }
        var bankAccount = bankAccountRepository.GetById(request.ReceiverAccountId.Value, bank.Id);
        if (bankAccount == null)
            return Error.NotFound(400, $"bank account {request.ReceiverAccountId} does not exist");

        if (bankAccount.Status != BankAccountStatus.Active && bankAccount.Status != BankAccountStatus.Freezed)
            return Error.AccessForbidden(400, "bank account has unappropriated status");

        if(bankAccount.UserAccountId != userAccountId)
            return Error.AccessForbidden(403,"invalid bank account id");
            
        bankAccount.Balance += request.Amount;
        bankAccountRepository.Update(bankAccount);
        var transaction = CreateTransaction(TransactionType.TopUp, null, request.ReceiverAccountId, request.Amount);
        transactionRepository.Add(transaction);

        return Result.Success();
    }
    
    public Result PerformWithdraw(Guid userAccountId,TransferRequest request, string bankName)
    {
        var result = withdrawValidator.Validate(request);
        if (!result.IsValid)
            return Error.Validation(400,
                string.Join(", ", result.Errors!.Select(e => e.ErrorMessage)));

        var bank = bankRepository.GetByName(bankName);
        if(bank == null)
            return Error.NotFound(400, "bank not found");
        if(!bankRepository.IsBank(bankName))
            return Error.NotFound(400, "bank not found");

        if(request.SenderAccountId == null)
            return Error.NotFound(400, "sender account not found");

        var bankAccount = bankAccountRepository.GetById(request.SenderAccountId.Value, bank.Id);
        if(bankAccount == null)
            return Error.NotFound(400, "bank account not found");
        
        if(userAccountId != bankAccount.UserAccountId)
            return Error.AccessForbidden(403,"invalid bank account id");
        result = withdrawAccountValidator.Validate(bankAccount);

        
        if (!result.IsValid)
            return Error.Validation(400,
                string.Join(", ", result.Errors!.Select(e => e.ErrorMessage)));
        if (bankAccount.Balance < request.Amount)
            return Error.Failure(400, "There are not enough funds to perform withdraw.");

        // connect to ATM and perform withdraw...

        bankAccount.Balance -= request.Amount;
        bankAccountRepository.Update(bankAccount);
        var transaction = CreateTransaction(TransactionType.Withdraw, request.SenderAccountId, null, request.Amount);
        transactionRepository.Add(transaction);
        return Result.Success();
    }


    private Result ValidateEmployee(Guid bankAccountId, Guid EnterpriseId, Guid bankId)
    {
        var bankAccount = bankAccountRepository.GetById(bankAccountId, bankId);
        if(bankAccount == null)
            return Error.NotFound(400, "bank account not found");
        if (bankAccount.Status != BankAccountStatus.Active && bankAccount.Status != BankAccountStatus.Freezed)
            return Error.Validation(400, "can't transfer salary to not active or frozen account");
        if (bankAccount.Type != BankAccountType.Salary)
            return Error.Validation(400, "invalid bank account type");
        if (bankAccount.EnterpriseId != EnterpriseId)
            return Error.Validation(400, "salary account not belongs to given enterprise");
        
        return Result.Success();
    }
    
    public Result PerformSalaryProjectTransaction(Guid specialistAccountId, string bankName, Guid enterpriseAccountId, Guid projectId)
    {
        var bank = bankRepository.GetByName(bankName);
        if(!bankRepository.IsBank(bankName))
            return Error.NotFound(400, "bank not found");
        
        var specialistAccount = userAccountRepository.GetById(specialistAccountId, bank!.Id);
        if(specialistAccount == null)
            return Error.NotFound(400, "specialist account not found");
        
        if(specialistAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "invalid specialist account id");
        
        var enterpriseAccount = bankAccountRepository.GetById(enterpriseAccountId, bank!.Id);
        if(enterpriseAccount == null)
            return Error.NotFound(400, "enterprise account not found");
        if(enterpriseAccount.Status != BankAccountStatus.Active)
            return Error.AccessForbidden(403, "enterprise account must be active");
        
        var project = salaryProjectRepository.GetById(projectId, bank!.Id);
        if (project == null)
        {
            return Error.NotFound(400, $"project {projectId} not found");
        }

        if (project.Status != VerificationStatus.Approved)
        {
            return Error.AccessForbidden(403, $"invalid project status {project.Status.ToString()}");
        }
        
        if(enterpriseAccount.Balance < project.Amount)
            return Error.Failure(400, "enterprise account has not enough funds");
        
        // check all employees

        var employeesAccounts =
            salaryProjectRequestRepository.GetApprovedSalaryProjectRequests(specialistAccount.EnterpriseId!.Value,
                project.ProjectId);

        foreach (var employee in employeesAccounts)
        {
            var resultValidation =
                ValidateEmployee(employee.SalaryAccountId, specialistAccount.EnterpriseId!.Value, bank.Id);
            if (!resultValidation.IsSuccess)
                return resultValidation.Error!;
        }

        foreach (var employeeRequest in employeesAccounts)
        {
            employeeRequest.SalaryAccount.Balance += employeeRequest.Salary;
            bankAccountRepository.Update(employeeRequest.SalaryAccount);
        }


        enterpriseAccount.Balance -= project.Amount;
        project.Status = VerificationStatus.Payed;
        salaryProjectRepository.Update(project);

        var transaction = CreateTransaction(TransactionType.Salary, enterpriseAccountId, projectId, project.Amount);
        transactionRepository.Add(transaction);
        
        return Result.Success();
    }

    private Result<BankAccount> VerifyBankAccount(Guid? bankAccountId, Guid bankId)
    {
        if(bankAccountId == null)
            return Error.NotFound(400, "bank account not found");
        var bankAccount = bankAccountRepository.GetById(bankAccountId.Value, bankId);
        if(bankAccount == null)
            return Error.NotFound(400, "bank account not found");
        return bankAccount;
    }
    
    public Result<Guid> CancelTransaction(Guid userAccountId, string bankName, Guid transactionId)
    {
        var bank = bankRepository.GetByName(bankName);
        if(!bankRepository.IsBank(bankName))
            return Error.NotFound(400, "bank not found");
        var userAccount = userAccountRepository.GetById(userAccountId, bank!.Id);
        if(userAccount == null)
            return Error.NotFound(400, "user account not found");
        if(userAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "user account must have status active");
        
        var transaction = transactionRepository.GetTransactionById(transactionId);
        if(transaction == null)
            return Error.NotFound(400, "transaction not found");
        var senderBankAccountId = transaction.SenderAccountId;
        var receiverBankAccountId = transaction.ReceiverAccountId;

        switch (transaction.Type)
        {
            case TransactionType.Transfer:
            {
                if(userAccount.UserRole < UserRole.Operator)
                    return Error.AccessForbidden(403, "user account must have role operator or higher to cancel transfer");
                var senderBankAccount = VerifyBankAccount(senderBankAccountId, bank.Id);
                if (!senderBankAccount.IsSuccess)
                    return senderBankAccount.Error!;
                var receiverBankAccount = VerifyBankAccount(receiverBankAccountId, bank.Id);
                if (!receiverBankAccount.IsSuccess)
                    return receiverBankAccount.Error!;

                // perform cancel
                if (transaction.Status == TransactionStatus.Cancelled)
                {
                    // cancel cancel
                    senderBankAccount.Value.Balance -= transaction.Amount;
                    receiverBankAccount.Value.Balance += transaction.Amount;
                    transaction.Status = TransactionStatus.Performed;
                }
                else
                {
                    // cancel
                    senderBankAccount.Value.Balance += transaction.Amount;
                    receiverBankAccount.Value.Balance -= transaction.Amount;
                    transaction.Status = TransactionStatus.Cancelled;
                }

                transactionRepository.Update(transaction);
                bankAccountRepository.Update(senderBankAccount.Value);
                bankAccountRepository.Update(receiverBankAccount.Value);
                return transaction.Id;
            }

            case TransactionType.Salary:
            {
                if(userAccount.UserRole < UserRole.Manager)
                    return Error.AccessForbidden(403, "user account must have role manager or higher to cancel transfer");
                var enterpriseBankAccountResult = VerifyBankAccount(senderBankAccountId, bank.Id);
                if(!enterpriseBankAccountResult.IsSuccess)
                    return enterpriseBankAccountResult.Error!;
                var enterpriseBankAccount = enterpriseBankAccountResult.Value;

                var projectId = transaction.SalaryProjectId;
                if(projectId == null)
                    return Error.NotFound(400, "project not found");

                var project = salaryProjectRepository.GetById(projectId.Value, bank!.Id);
                if(project == null)
                    return Error.NotFound(400, "project not found");
                // get all salary project requests
                
                var employeesSalaryProjectRequests =
                    salaryProjectRequestRepository.GetApprovedSalaryProjectRequests(enterpriseBankAccount.EnterpriseId!.Value, project.ProjectId);

                foreach (var projectRequest in employeesSalaryProjectRequests)
                {
                    var resultValidation =
                        VerifyBankAccount(projectRequest.SalaryAccountId, bank.Id);
                    if (!resultValidation.IsSuccess)
                        return resultValidation.Error!;
                }

                if (transaction.Status == TransactionStatus.Cancelled)
                {
                    foreach (var employeeRequest in employeesSalaryProjectRequests)
                    {
                        employeeRequest.SalaryAccount!.Balance += employeeRequest.Salary;
                        bankAccountRepository.Update(employeeRequest.SalaryAccount);
                    }
                    enterpriseBankAccount.Balance -= transaction.Amount;
                    bankAccountRepository.Update(enterpriseBankAccount);
                    transaction.Status = TransactionStatus.Performed;
                }
                else
                {
                    foreach (var employeeRequest in employeesSalaryProjectRequests)
                    {
                        employeeRequest.SalaryAccount!.Balance -= employeeRequest.Salary;
                        bankAccountRepository.Update(employeeRequest.SalaryAccount);
                    }
                    enterpriseBankAccount.Balance += transaction.Amount;
                    bankAccountRepository.Update(enterpriseBankAccount);
                    transaction.Status = TransactionStatus.Cancelled;
                }

                transactionRepository.Update(transaction);

                return transaction.Id;
            }
            case TransactionType.Withdraw:
            {
                if(userAccount.UserRole < UserRole.Administrator)
                    return Error.AccessForbidden(403, "user account must have role administrator or higher to cancel transfer");
                var senderBankAccountResult = VerifyBankAccount(senderBankAccountId, bank.Id);
                if(!senderBankAccountResult.IsSuccess)
                    return senderBankAccountResult.Error!;
                if (transaction.Status == TransactionStatus.Cancelled)
                {
                    senderBankAccountResult.Value.Balance -= transaction.Amount;
                    transaction.Status = TransactionStatus.Performed;
                }
                else
                {
                    senderBankAccountResult.Value.Balance += transaction.Amount;
                    transaction.Status = TransactionStatus.Cancelled;
                }
                bankAccountRepository.Update(senderBankAccountResult.Value);
                transactionRepository.Update(transaction);
                return transaction.Id;
            }
            case TransactionType.TopUp:
            {
                if(userAccount.UserRole < UserRole.Operator)
                    return Error.AccessForbidden(403, "user account must have role operator or higher to cancel transfer");
                var receiverBankAccountResult = VerifyBankAccount(receiverBankAccountId, bank.Id);
                if(!receiverBankAccountResult.IsSuccess)
                    return receiverBankAccountResult.Error!;
                if (transaction.Status == TransactionStatus.Cancelled)
                {
                    receiverBankAccountResult.Value.Balance += transaction.Amount;
                    transaction.Status = TransactionStatus.Performed;
                }
                else
                {
                    receiverBankAccountResult.Value.Balance -= transaction.Amount;
                    transaction.Status = TransactionStatus.Cancelled;
                }
                bankAccountRepository.Update(receiverBankAccountResult.Value);
                transactionRepository.Update(transaction);
                return transaction.Id;
            }
            default:
            {
                return Error.Failure(400, "cancel od this operation type is not supported");
            }
        }
    }
}