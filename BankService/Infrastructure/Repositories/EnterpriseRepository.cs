using BankService.Domain.Entities;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class EnterpriseRepository(ApplicationContext db) : IEnterpriseRepository
{
    public Enterprise? GetById(Guid id)
    {
        return db.Enterprises.FirstOrDefault(u => u.Id == id);
    }

    public void Add(Enterprise enterprise)
    {
        db.Enterprises.Add(enterprise);
        db.SaveChanges();
    }

    public void Update(Enterprise enterprise)
    {
        db.Entry(enterprise).State = EntityState.Modified;
        db.SaveChanges();
    }

    public Enterprise? GetByName(string? name)
    {
        return db.Enterprises.FirstOrDefault(u => u.Name == name);
    }

    public bool IsBank(string name)
    {
        var isBank = db.Enterprises.OfType<Bank>().Any(b => b.Name == name);
        return isBank;
    }

    public bool IsBank(Guid id)
    {
        var isBank = db.Enterprises.Any(b => b.Id == id && b is Bank);
        return isBank;
    }

    public IEnumerable<string> GetAllBanks()
    {
        return db.Enterprises.OfType<Bank>().Select(b => b.Name);
    }

    public IEnumerable<string> GetAllEnterprise()
    {
        return db.Enterprises.Select(e => e.Name);
    }
}