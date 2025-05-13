using BankService.Domain.Entities;

namespace BankService.Domain.Interfaces.IRepositories;

public interface IUserRepository
{
    User? GetById(Guid id);
    User? FindBySameData(User user);
    User? FindByUniqueData(User user);

    User? FindByPassport(string data);
    void Add(User user);
    void Update(User user);
}