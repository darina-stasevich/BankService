using System.ComponentModel.DataAnnotations;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.Loans;

namespace BankService.Domain.Entities;

public class Bank : Enterprise
{

    [Required(ErrorMessage = "Bank name is required")]
    
    // Navigation properties
    public List<UserAccount>? UserAccounts { get; set; }
    public List<SalaryProjectRequest>? SalaryProjectRequests { get; set; }
    public List<Loan>? Loans { get; set; }
    public List<LoanRequest>? LoanRequests { get; set; }
    public List<BankAccount>? BankAccounts { get; set; }
}