using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;

namespace BankService.Domain.Interfaces.IRepositories;

public interface IBankAccountRepository
{
    BankAccount? GetById(Guid id, Guid bankId);
    void Add(BankAccount account);
    void Update(BankAccount account);
    void Delete(BankAccount account);
    public IEnumerable<BankAccount> GetAccounts(Guid userAccountId, BankAccountStatus bankAccountStatus);

    public IEnumerable<BankAccount> GetEnterpriseAccounts(Guid enterpriseId, Guid bankId,
        BankAccountStatus bankAccountStatus);
    public IEnumerable<BankAccount> GetAccountsForBank(Guid bankId, BankAccountStatus bankAccountStatus);

}