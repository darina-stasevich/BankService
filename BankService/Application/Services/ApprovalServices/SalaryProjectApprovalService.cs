using BankService.Domain.Entities;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class SalaryProjectApprovalService(
    IApprovalStrategy approvalStrategy,
    IEnterpriseRepository enterpriseRepository,
    IUserAccountRepository userAccountRepository,
    ISalaryProjectRequestRepository salaryProjectRequestRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IUserRepository userRepository,
    INotificationService notificationService) : ISalaryProjectApprovalService
{
    
    public Result<Guid> ApproveProject(Guid approverAccountId, string bankName, string projectId)
    {
        if(!enterpriseRepository.IsBank(bankName))
            return Error.NotFound(400, $"bank {bankName} not found");
        var bank = enterpriseRepository.GetByName(bankName);
        return approvalStrategy.ApproveProject(approverAccountId, bank!.Id, projectId);
    }
    
    private Result<UserAccount> ValidateAccount(Guid accountId, Guid bankId)
    {
        Console.WriteLine($"id : {accountId} bankId : {bankId}");
        var approverAccount = userAccountRepository.GetById(accountId, bankId);
        if (approverAccount == null)
            return Error.NotFound(400,
                $"approver account with id: {accountId} not found");
        if (approverAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "only active users can approve requests");
        if (approverAccount.UserRole != UserRole.ExternalSpecialist)
            return Error.AccessForbidden(403, "only employees of bank can approve request");
        return approverAccount;

    }

    private Result<SalaryProjectRequest> ValidateRequest(Guid requestId, Guid bankId)
    {
        var request = salaryProjectRequestRepository.GetById(requestId, bankId);
        if (request == null)
            return Error.NotFound(400, $"request with id: {requestId} not found");
        return request;
    }
    public Result<Guid> ApproveSalaryProjectRequest(Guid approverAccountId, Guid salaryProjectRequestId, string bankName)
    {
        if(!enterpriseRepository.IsBank(bankName))
            return Error.NotFound(400, $"bank {bankName} not found");
        var bank = enterpriseRepository.GetByName(bankName);
        
        var accountValidationResult = ValidateAccount(approverAccountId, bank!.Id);
        if(!accountValidationResult.IsSuccess)
            return accountValidationResult.Error;
        
        var requestValidationResult = ValidateRequest(salaryProjectRequestId, bank.Id);
        if(!requestValidationResult.IsSuccess)
            return requestValidationResult.Error;
        
        var request = requestValidationResult.Value;

        if (request.Status == VerificationStatus.Approved)
        {
            return Error.Conflict(400, "request already approved");
        }
        var employeeUserAccount = userAccountRepository.GetById(request.EmployeeAccountId, bank.Id);
        var userData = userRepository.GetById(employeeUserAccount.UserId);
        if (userData == null)
            return Error.NotFound(400, "user not found");
        
        var salaryProject = salaryProjectRepository.GetByProjectId(request.ProjectId, bank.Id, request.EnterpriseId.Value);
        if (salaryProject != null)
        {
            request.Status = VerificationStatus.Rejected;
            salaryProjectRequestRepository.Update(request);

            notificationService.SendNotification(userData.Email, $"Salary project request {request.ProjectId} rejected", null);
            return Error.Conflict(400,
                $"can't approve salary project request. Project {request.ProjectId} already formed");
        }

        request.Status = VerificationStatus.Approved;
        salaryProjectRequestRepository.Update(request);

        notificationService.SendNotification(userData.Email, $"Salary project request {request.ProjectId} approved", null);
        
        return request.Id;
    }

    public Result<Guid> RejectSalaryProjectRequest(Guid approverAccountId, Guid salaryProjectRequestId, string bankName)
    {
        if(!enterpriseRepository.IsBank(bankName))
            return Error.NotFound(400, $"bank {bankName} not found");
        var bank = enterpriseRepository.GetByName(bankName);
        
        var accountValidationResult = ValidateAccount(approverAccountId, bank.Id);
        if(!accountValidationResult.IsSuccess)
            return accountValidationResult.Error;
        
        var requestValidationResult = ValidateRequest(salaryProjectRequestId, bank.Id);
        if(!requestValidationResult.IsSuccess)
            return requestValidationResult.Error;
        
        var request = requestValidationResult.Value;
        if (request.Status == VerificationStatus.Rejected)
        {
            return Error.Conflict(400, "request already rejected");
        }
        
        var employeeUserAccount = userAccountRepository.GetById(request.EmployeeAccountId, bank.Id);
        var userData = userRepository.GetById(employeeUserAccount.UserId);
        if (userData == null)
            return Error.NotFound(400, "user not found");

        var salaryProject = salaryProjectRepository.GetByProjectId(request.ProjectId, bank.Id, request.EnterpriseId.Value);
        if (salaryProject != null && request.Status == VerificationStatus.Approved)
        {
            return Error.Conflict(400,
                $"can't reject salary project request. Project {request.ProjectId} already formed and request already approved");
        } 

        request.Status = VerificationStatus.Rejected;
        salaryProjectRequestRepository.Update(request);
        notificationService.SendNotification(userData.Email, $"Salary project request {request.ProjectId} rejected", null);

        return request.Id;
    }
    
}