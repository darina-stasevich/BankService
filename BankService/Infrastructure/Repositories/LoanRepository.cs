using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class LoanRepository (ApplicationContext db): ILoanRepository
{
    public Loan? GetLoanById(Guid id)
    {
        return db.Loans.FirstOrDefault(x => x.Id == id);
    }

    public Installment? GetInstallmentById(Guid id)
    {
        return db.Loans.OfType<Installment>().FirstOrDefault(x => x.Id == id);
    }

    public Credit? GetCreditById(Guid id)
    {
        return db.Loans.OfType<Credit>().FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Loan> GetLoansByUserAccountId(Guid userAccountId)
    {
        return db.Loans.Include(l => l.BankAccount)
            .ThenInclude(ba => ba.UserAccount)
            .Where(l => l.BankAccount.UserAccount.Id == userAccountId);
    }

    public void Add(Loan loan)
    {
        db.Loans.Add(loan);
        db.SaveChanges();
    }

    public void Update(Loan loan)
    {
        db.Loans.Update(loan);
        db.SaveChanges();
    }
}