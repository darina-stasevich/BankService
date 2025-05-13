using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.BankAccounts;

public class SalaryAccount : BankAccount
{
    public override BankAccountType Type => BankAccountType.Salary;

    public override bool CreditAllowed { get; } = false;
    public override bool InstallmentAllowed { get; } = false;
    
}