using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities;

public class SalaryProject
{
    public Guid Id { get; set; }
    
    [ForeignKey("Enterprise")]
    public Guid EnterpriseId { get; set; } // преприятие, чей проект
    
    [ForeignKey("SpecialistUserAccount")]
    public Guid SpecialistId { get; set; } // аккаунт подавшего специалиста
    
    [ForeignKey("Bank")]
    public Guid BankId { get; set; } // банк, в котором регистрируется
    public string ProjectId { get; set; } // просто id проекта
    public DateTime SendRequestDate { get; set; }
    public VerificationStatus Status { get; set; }
    public decimal Amount { get; set; }
    // navigation properties
    
    public Enterprise? Enterprise { get; set; }
    public UserAccount? SpecialistUserAccount { get; set; }
    public Bank? Bank { get; set; }
}