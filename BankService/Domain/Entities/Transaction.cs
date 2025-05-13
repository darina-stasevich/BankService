using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankService.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }
    
    [ForeignKey("SenderBankAccount")] public Guid? SenderAccountId { get; set; }
    
    [ForeignKey("ReceiverBankAccount")] public Guid? ReceiverAccountId { get; set; }
    
    [ForeignKey("SalaryProject")] public Guid? SalaryProjectId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionStatus Status { get; set; }
    
    // navigation properties
    
    public BankAccount? SenderBankAccount { get; set; }
    public BankAccount? ReceiverBankAccount { get; set; }
    public SalaryProject? SalaryProject { get; set; }
}