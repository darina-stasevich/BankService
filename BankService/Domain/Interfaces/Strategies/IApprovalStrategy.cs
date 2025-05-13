using BankService.Domain.Entities;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces;

public interface IApprovalStrategy
{
    public Result<Guid> ApproveProject(Guid approverUserAccountId, Guid bankId, string projectId);
}