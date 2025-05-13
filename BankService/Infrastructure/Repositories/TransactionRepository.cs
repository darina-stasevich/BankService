using BankService.Domain.Entities;
using BankService.Domain.Interfaces;
using BankService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankService.Infrastructure.Repositories;

public class TransactionRepository(ApplicationContext db) : ITransactionRepository
{
    public Transaction? GetTransactionById(Guid id)
    {
        return db.Transactions.FirstOrDefault(x => x.Id == id);
    }

    public void Add(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        db.SaveChanges();
    }

    public void Update(Transaction transaction)
    {
        db.Transactions.Update(transaction);
        db.SaveChanges();
    }

    public IEnumerable<Transaction> GetTransactions(Guid bankId)
    {
        return db.Transactions
            .Include(x => x.ReceiverBankAccount)
            .Include(x => x.SenderBankAccount)
            .Where(x =>
                (x.SenderBankAccount != null && x.SenderBankAccount.BankId == bankId) ||
                (x.ReceiverBankAccount != null && x.ReceiverBankAccount.BankId == bankId));
    }
}