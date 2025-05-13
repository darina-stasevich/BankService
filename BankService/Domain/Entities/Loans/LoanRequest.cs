using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.Loans;

public class LoanRequest
{
    public Guid Id { get; set; }
    [ForeignKey("Banks")] public Guid BankId { get; set; }
    [ForeignKey("BankAccount")] public Guid BankAccountId { get; set; }
    public LoanType LoanType { get; set; }
    public decimal TotalAmount { get; set; }
    public int TermMonths { get; set; }
    public decimal InterestRate { get; set; }
    
    public VerificationStatus VerificationStatus { get; set; }
    
    // navigation properties
    
    public Bank? Bank { get; set; }
    public BankAccount? BankAccount { get; set; }

}