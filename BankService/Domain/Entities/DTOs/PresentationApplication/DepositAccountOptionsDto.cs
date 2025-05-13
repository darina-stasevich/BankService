namespace BankService.Domain.Entities.DTOs;

public class DepositAccountOptionsDto
{
    public bool IsEarlyWithdrawalAllowed { get; set; } = false;
    public decimal InterestRate { get; set; }
    
    public DateTime MaturityDate { get; set; }
}