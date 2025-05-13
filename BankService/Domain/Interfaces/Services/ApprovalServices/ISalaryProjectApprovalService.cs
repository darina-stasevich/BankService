using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.ApprovalServices;

public interface ISalaryProjectApprovalService
{
    public Result<Guid> ApproveSalaryProjectRequest(Guid approverAccountId, Guid salaryProjectRequestId, string bankName);
    public Result<Guid> RejectSalaryProjectRequest(Guid approverAccountId, Guid salaryProjectRequestId, string bankName);
    public Result<Guid> ApproveProject(Guid approverAccountId, string bankName, string projectId);
}