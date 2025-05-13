using BankService.Domain.Entities;

namespace BankService.Domain.Interfaces.IRepositories;

public interface IEnterpriseRepository
{
    public Enterprise? GetById(Guid id);
    public void Add(Enterprise enterprise);
    public void Update(Enterprise enterprise);
    public Enterprise? GetByName(string? name);
    public bool IsBank(string name);
    public bool IsBank(Guid id);
    public IEnumerable<string> GetAllBanks();
    
    public IEnumerable<string> GetAllEnterprise();
}