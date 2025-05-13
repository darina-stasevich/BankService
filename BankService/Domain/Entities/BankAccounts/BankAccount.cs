using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.BankAccounts;

public abstract class BankAccount
{
    public Guid Id { get; set; }
    [Required] [ForeignKey("UserAccount")] public Guid? UserAccountId { get; set; }
    [Required] [ForeignKey("Bank")] public Guid BankId { get; set; }
    
    [ForeignKey("Enterprise")] public Guid? EnterpriseId { get; set; }
    public BankAccountStatus Status { get; set; }
    
    [NotMapped]
    public abstract BankAccountType Type { get; }
    public decimal Balance { get; set; } = 0;

    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime? FrozenTill { get; set; } = null;
    public DateTime? BlockedDate { get; set; } = null;
    public virtual bool CreditAllowed { get; } = true;
    public virtual bool InstallmentAllowed { get; } = true;
    
    // navigation properties

    public ICollection<Loan>? Loans { get; set; }
    public ICollection<LoanRequest> LoanRequests { get; set; }
    public UserAccount? UserAccount { get; set; }
    public Bank? Bank { get; set; }
    public Enterprise? Enterprise { get; set; }
}