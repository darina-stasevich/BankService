using System.Runtime.InteropServices.JavaScript;
using BankService.Domain.Entities;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Results;
using BankService.Infrastructure.Repositories;

namespace BankService.Application.ApprovalStrategy;

public class OperatorApprovalStrategy(
    IUserAccountRepository userAccountRepository,
    ISalaryProjectRepository salaryProjectRepository) : IApprovalStrategy
{
    private Result<UserAccount> ValidateAccount(Guid accountId, Guid bankId)
    {
        var approverAccount = userAccountRepository.GetById(accountId, bankId);
        if(approverAccount == null)
            return Error.NotFound(400, $"approver account with id: {approverAccount?.Id} not found");
        if (approverAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "only active users can approve requests");
        if (approverAccount.UserRole < UserRole.Operator)
            return Error.AccessForbidden(403, "Not enough rights to approve request");

        return approverAccount;

    }

    public Result<Guid> ApproveProject(Guid approverUserAccountId, Guid bankId, string projectId)
    {
        var accountValidationResult = ValidateAccount(approverUserAccountId, bankId);
        if (!accountValidationResult.IsSuccess)
            return accountValidationResult.Error;
        var projectGuidResult = Guid.TryParse(projectId, out var projectGuid);
        if(!projectGuidResult)
            return Error.Validation(400, "Project ID is invalid");
        var projectRequest = salaryProjectRepository.GetById(projectGuid, bankId);
        if(projectRequest!.Status == VerificationStatus.Approved)
            return Error.Conflict(400, "Project already approved");
        projectRequest.Status = VerificationStatus.Approved;
        salaryProjectRepository.Update(projectRequest);
        return projectRequest.Id;
    }
}