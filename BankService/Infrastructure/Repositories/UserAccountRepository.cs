using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class UserAccountRepository(ApplicationContext db) : IUserAccountRepository
{
    public UserAccount? GetById(Guid id, Guid bankId)
    {
        return db.UserAccounts.FirstOrDefault(u => u.Id == id && u.BankId == bankId);
    }
    public UserAccount? GetByLogin(string login, Guid bankId)
    {
        return  db.UserAccounts.FirstOrDefault(u => u.Login == login && u.BankId == bankId);
    }

    public UserAccount? GetByUniqueData(UserAccount userAccount)
    {
        return db.UserAccounts.FirstOrDefault(u => u.UserId == userAccount.UserId &&
                                                   u.UserRole == userAccount.UserRole &&
                                                   u.BankId == userAccount.BankId);
    }

    public void Add(UserAccount account)
    {
        Console.WriteLine($"account {account.UserId} {account.BankId} {account.UserRole.ToString()} {account.Status.ToString()}");
        db.UserAccounts.Add(account);
        db.SaveChanges();
    }

    public void Update(UserAccount account)
    {
        db.Entry(account).State = EntityState.Modified;
        db.SaveChanges();
    }

    public IEnumerable<UserAccount> GetUserAccounts(Guid bankId)
    {
        return db.UserAccounts.Where(u => u.BankId == bankId).ToList();
    }
    
    public IEnumerable<UserAccount> GetUserAccounts(Guid bankId, VerificationStatus status)
    {
        return db.UserAccounts.Where(u => u.BankId == bankId && u.Status == status);
    }
}