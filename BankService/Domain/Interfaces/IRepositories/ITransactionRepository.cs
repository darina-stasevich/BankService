using BankService.Domain.Entities;

namespace BankService.Domain.Interfaces;

public interface ITransactionRepository
{
    public Transaction? GetTransactionById(Guid id);
    public void Add(Transaction transaction);
    public void Update(Transaction transaction);
    
    public IEnumerable<Transaction> GetTransactions(Guid bankId);
}