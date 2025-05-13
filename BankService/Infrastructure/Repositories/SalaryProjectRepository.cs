using System.Runtime.InteropServices.ComTypes;
using BankService.Domain.Entities;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class SalaryProjectRepository (ApplicationContext db) : ISalaryProjectRepository
{
    public SalaryProject? GetById(Guid id, Guid bankId)
    {
        return db.SalaryProjects.FirstOrDefault(x => x.Id == id && x.BankId == bankId);
    }

    public void Add(SalaryProject salaryProject)
    {
        db.SalaryProjects.Add(salaryProject);
        db.SaveChanges();
    }

    public void Update(SalaryProject salaryProject)
    {
        db.Entry(salaryProject).State = EntityState.Modified;
        db.SaveChanges();
    }

    public SalaryProject? GetByProjectId(string projectId, Guid bankId, Guid enterpriseId)
    {
        return db.SalaryProjects.FirstOrDefault(x => x.BankId == bankId && x.EnterpriseId == enterpriseId && x.ProjectId == projectId);
    }

    public IEnumerable<SalaryProject> GetBankProjects(Guid bankId)
    {
        return db.SalaryProjects.Where(x => x.BankId == bankId);
    }
}