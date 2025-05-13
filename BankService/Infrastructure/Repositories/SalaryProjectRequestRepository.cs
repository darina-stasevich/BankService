using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class SalaryProjectRequestRepository(ApplicationContext db) : ISalaryProjectRequestRepository
{
    public SalaryProjectRequest? GetById(Guid id, Guid bankId)
    {
        return db.SalaryProjectRequests.FirstOrDefault(x => x.Id == id && x.BankId == bankId);
    }
    public void Add(SalaryProjectRequest request)
    {
        db.SalaryProjectRequests.Add(request);
        db.SaveChanges();
    }

    public void Update(SalaryProjectRequest request)
    {
        Console.WriteLine("Updating salary project request");
        db.SalaryProjectRequests.Update(request);
        db.SaveChanges();
    }

    public IEnumerable<SalaryProjectRequest> GetSalaryProjectRequests(Guid enterpriseId, VerificationStatus status)
    {
        return db.SalaryProjectRequests.Include(r => r.EmployeeAccount).ThenInclude(r => r.User).Where(x => x.EnterpriseId == enterpriseId && x.Status == status);
    }

    public IEnumerable<SalaryProjectRequest> GetApprovedSalaryProjectRequests(Guid enterpriseId, string projectId)
    {
        return db.SalaryProjectRequests.Include(x => x.SalaryAccount).Where(x => x.EnterpriseId == enterpriseId && x.ProjectId == projectId && x.Status == VerificationStatus.Approved);
    }
}