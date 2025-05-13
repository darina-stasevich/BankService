using BankService.Domain.Enums;

namespace BankService.Domain.Entities.DTOs;

public class LoanDataDTO
{
    public LoanType LoanType { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateTime NextPaymentDate { get; set; }
}