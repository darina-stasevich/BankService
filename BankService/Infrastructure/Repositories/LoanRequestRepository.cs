using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class LoanRequestRepository(ApplicationContext db) : ILoanRequestRepository
{
    public LoanRequest? GetById(Guid id, Guid bankId)
    {
        return db.LoanRequests.FirstOrDefault(x => x.Id == id && x.BankId == bankId);
    }

    public void Add(LoanRequest loanRequest)
    {
        db.LoanRequests.Add(loanRequest);
        db.SaveChanges();
    }

    public void Update(LoanRequest loanRequest)
    {
        db.Entry(loanRequest).State = EntityState.Modified;
        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        db.LoanRequests.Where(x => x.Id == id).ExecuteDelete();
        db.SaveChanges();
    }

    public IEnumerable<LoanRequest> GetLoanRequests(Guid bankId, VerificationStatus status)
    {
        return db.LoanRequests.Where(x => x.BankId == bankId && x.VerificationStatus == status);
    }
}