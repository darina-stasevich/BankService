using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities;

public class UserAccount
{
    public Guid Id { get; set; }

    [ForeignKey("User")] public required Guid UserId { get; set; }
    [ForeignKey("Bank")] public required Guid BankId { get; set; }
    [ForeignKey("Enterprise")]public Guid? EnterpriseId { get; set; } // for specialists

    public required UserRole UserRole { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public VerificationStatus Status { get; set; }
    public DateTime ChangedStatusAt { get; set; }
    
    // navigation properties
    public Bank? Bank { get; set; }
    public User? User { get; set; }
    public Enterprise? Enterprise { get; set; }
    public List<SalaryProjectRequest>? SalaryProjectRequests { get; set; }
    public ICollection<BankAccount>? BankAccounts { get; set; }
}