using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces;

public interface IUserContext
{
    public UserRole? Role { get; }
    public string CurrentBank { get; }
    public string? CurrentEnterprise { get;  }
    public void InitializeRole(UserRole role);
    public void InitializeBank(string? bank);
    public void InitializeEnterprise(string enterprise);
    public void Clear();
    
    public bool IsAuthenticated { get; }
    public Result<Guid> UserAccountId { get; }
}