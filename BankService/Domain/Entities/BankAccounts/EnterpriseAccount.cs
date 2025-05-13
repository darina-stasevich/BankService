using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.BankAccounts;

public class EnterpriseAccount : BankAccount
{
    public override BankAccountType Type => BankAccountType.Enterprise;

    public override bool CreditAllowed { get; } = false;
    public override bool InstallmentAllowed { get; } = false;
    
    // navigation properties
    
    //    public List<SalaryProjectRequest>? SalaryProjectRequests { get; set; }
}