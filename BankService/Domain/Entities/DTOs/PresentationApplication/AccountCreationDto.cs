using BankService.Domain.Enums;

namespace BankService.Domain.Entities.DTOs;

public class AccountCreationDto
{
    public Guid? UserAccountId { get; set; }
    public string Bank { get; set; }
    public string? Enterprise { get; set; }
    
    public DepositAccountOptionsDto? DepositAccountOptionsDto { get; set; }
    public BankAccountType Type { get; set; }
}