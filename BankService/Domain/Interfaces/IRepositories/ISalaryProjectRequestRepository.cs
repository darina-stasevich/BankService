using BankService.Domain.Entities;
using BankService.Domain.Enums;

namespace BankService.Domain.Interfaces.IRepositories;

public interface ISalaryProjectRequestRepository
{
    public SalaryProjectRequest? GetById(Guid id, Guid bankId);
    public void Add(SalaryProjectRequest request);
    
    public void Update(SalaryProjectRequest request);
    
    public IEnumerable<SalaryProjectRequest> GetSalaryProjectRequests(Guid enterpriseId, VerificationStatus status);
    
    public IEnumerable<SalaryProjectRequest> GetApprovedSalaryProjectRequests(Guid enterpriseId, string projectId);
}