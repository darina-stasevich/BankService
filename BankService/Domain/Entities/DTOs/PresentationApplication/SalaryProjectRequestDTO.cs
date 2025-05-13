using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;

namespace BankService.Domain.Entities.DTOs;

public class SalaryProjectRequestDTO
{
    [ForeignKey("EnterpriseId")]
    public Guid? EnterpriseId { get; set; }
    public string BankName { get; set; }
    public Guid BankId { get; set; }
    public Guid SalaryAccountId { get; set; }
    public string ProjectId { get; set; }
    public decimal Amount { get; set; }
}