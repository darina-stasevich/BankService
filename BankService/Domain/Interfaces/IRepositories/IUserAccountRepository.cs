using BankService.Domain.Entities;
using BankService.Domain.Enums;

namespace BankService.Domain.Interfaces.IRepositories;

public interface IUserAccountRepository
{
    UserAccount? GetById(Guid id, Guid bankId);
    UserAccount? GetByLogin(string login, Guid bankId);
    UserAccount? GetByUniqueData(UserAccount userAccount);
    IEnumerable<UserAccount> GetUserAccounts(Guid bankId);
    IEnumerable<UserAccount> GetUserAccounts(Guid bankId, VerificationStatus status);
    void Add(UserAccount account);
    void Update(UserAccount account);
}