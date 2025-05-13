using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities.BankAccounts;

public class CurrentAccount : BankAccount
{
    public List<Credit>? Credits { get; set; }
    public List<Installment>? Installments { get; set; }

    public override BankAccountType Type => BankAccountType.Current;
}