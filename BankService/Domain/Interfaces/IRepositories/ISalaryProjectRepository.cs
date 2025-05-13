using BankService.Domain.Entities;

namespace BankService.Domain.Interfaces.IRepositories;

public interface ISalaryProjectRepository
{
    public SalaryProject? GetById(Guid id, Guid bankId);
    public void Add(SalaryProject salaryProject);
    public void Update(SalaryProject salaryProject);
    public SalaryProject? GetByProjectId(string projectId, Guid bankId, Guid enterpriseId);
    
    public IEnumerable<SalaryProject> GetBankProjects(Guid bankId);
}