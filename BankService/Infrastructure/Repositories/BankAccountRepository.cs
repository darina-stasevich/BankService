using System.Diagnostics;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class BankAccountRepository(ApplicationContext db) : IBankAccountRepository
{
    public BankAccount? GetById(Guid id, Guid bankId)
    {
        return db.BankAccounts.FirstOrDefault(u => u.Id == id && u.BankId == bankId);
    }

    public void Add(BankAccount account)
    {
        db.BankAccounts.Add(account);
        db.SaveChanges();
    }

    public void Update(BankAccount account)
    {
        db.Entry(account).State = EntityState.Modified;
        Console.WriteLine($"update account {account.Id}");
        db.SaveChanges();
    }

    public void Delete(BankAccount account)
    {
        db.Entry(account).State = EntityState.Deleted;
        db.SaveChanges();
    }

    public IEnumerable<BankAccount> GetAccounts(Guid userAccountId, BankAccountStatus bankAccountStatus)
    {
        //var data = db.BankAccounts.ToList().Where(b => b.Status == bankAccountStatus && b.UserAccountId == userAccountId)
        //        .Select(b => new BankAccountDTO { Balance = b.Balance, Id = b.Id, AccountType = b.Type });
        var data = db.BankAccounts.Where(x => x.UserAccountId == userAccountId && x.Status == bankAccountStatus);
        return data;
    }

    public IEnumerable<BankAccount> GetEnterpriseAccounts(Guid enterpriseId, Guid bankId, BankAccountStatus bankAccountStatus)
    {
        
        return db.BankAccounts.Where(x => x.EnterpriseId == enterpriseId && x.BankId == bankId && x.Status == bankAccountStatus);
    }

    public IEnumerable<BankAccount> GetAccountsForBank(Guid bankId, BankAccountStatus bankAccountStatus)
    {
        var data = db.BankAccounts.Where(x => x.BankId == bankId && x.Status == bankAccountStatus);

        return data;
    }
}