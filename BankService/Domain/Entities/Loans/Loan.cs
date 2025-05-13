using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.Loans;

public class Loan
{
    public Guid Id { get; set; }

    [ForeignKey("BankAccount")] 
    public Guid BankAccountId { get; set; } // Связанный текущий счет
    [ForeignKey("Bank")] public Guid BankId { get; set; } // банк, который должен выдать кредит
    [Required] public decimal TotalAmount { get; set; } // Общая сумма кредита (тело + проценты)
    [Required] public int TermMonths { get; set; } // Срок кредита в месяцах
    [Required] public decimal PaidAmount { get; set; } = 0; // Уже выплаченная сумма
    [Required] public DateTime NextPaymentDate { get; set; } // Дата следующего платежа
    [Required] public Decimal InterestRate { get; set; }
    // Остаток к выплате (вычисляемое свойство)
    [NotMapped] public decimal RemainingAmount => TotalAmount - PaidAmount;
    
    // navigation properties
    public BankAccount? BankAccount { get; set; }
    public Bank? Bank { get; set; }
}