using System.ComponentModel.DataAnnotations;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.DTOs;

public class LoanRequestDTO
{
    [Required] public decimal TotalAmount { get; set; }

    [Required] public int TermMonths { get; set; }

    public Guid? BankAccountId { get; set; }

    public LoanType LoanType { get; set; }
    
    public decimal InterestRate { get; set; }
}