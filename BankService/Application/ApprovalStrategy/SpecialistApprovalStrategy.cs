using System.Runtime.InteropServices.JavaScript;
using BankService.Domain.Entities;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Results;

namespace BankService.Application.ApprovalStrategy;

public class SpecialistApprovalStrategy(
    IUserAccountRepository userAccountRepository,
    ISalaryProjectRequestRepository salaryProjectRequestRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IUserRepository userRepository,
    INotificationService notificationService) : IApprovalStrategy
{
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
        if (request.Status != VerificationStatus.Pending)
            return Error.AccessForbidden(403, "specialist can approve only pending requests");
        return request;
    }
    
    
    public Result<Guid> ApproveProject(Guid approverUserAccountId, Guid bankId, string projectId)
    {
        var accountValidationResult = ValidateAccount(approverUserAccountId, bankId);
        if (!accountValidationResult.IsSuccess)
            return accountValidationResult.Error!;
        var userAccount = userAccountRepository.GetById(approverUserAccountId, bankId);
       var project = CreateSalaryProject(approverUserAccountId, bankId, projectId, userAccount!.EnterpriseId!.Value);
        var request = salaryProjectRepository.GetByProjectId(projectId, bankId, userAccount.Id);
        if (request != null)
            return Error.Conflict(400, $"project {projectId} already formed");
        var projectRequests =
            salaryProjectRequestRepository.GetSalaryProjectRequests(project.EnterpriseId,
                VerificationStatus.Pending).Where(x => x.ProjectId == projectId);
        if(projectRequests.Count() != 0)
            return Error.Conflict(400, $"project {projectId} can not be approved before all requests are approved");

        var approvedProjectRequestsAmount =
            salaryProjectRequestRepository.GetSalaryProjectRequests(project.EnterpriseId,
                VerificationStatus.Approved).Where(x => x.ProjectId == projectId).Sum(x => x.Salary);
        project.Amount = approvedProjectRequestsAmount;
        salaryProjectRepository.Add(project);
        return project.Id;
    }

    private SalaryProject CreateSalaryProject(Guid senderUserAccountId, Guid bankId, string projectId, Guid enterpriseId)
    {
        var project = new SalaryProject
        {
            SpecialistId = senderUserAccountId,
            ProjectId = projectId,
            BankId = bankId,
            EnterpriseId = enterpriseId,
            SendRequestDate = DateTime.Now,
            Status = VerificationStatus.Pending
        };
        return project;
    }
}