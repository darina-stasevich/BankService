using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.BankAccounts;

public class DepositAccount : BankAccount
{
    public decimal InterestRate { get; set; }
    public DateTime MaturityDate { get; set; }
    public bool IsEarlyWithdrawalAllowed { get; set; }
    public override BankAccountType Type => BankAccountType.Deposit;

    public override bool CreditAllowed { get; } = false;
    public override bool InstallmentAllowed { get; } = false;
}