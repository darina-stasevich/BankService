using BankService.Domain.Enums;

namespace BankService.Domain.Entities.DTOs;

public class BankAccountDataDTO
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public BankAccountType AccountType { get; set; }
}