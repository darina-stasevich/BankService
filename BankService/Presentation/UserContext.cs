using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Results;

namespace BankService.Presentation;

public class UserContext (ITokenService tokenService,
    ITokenStorage tokenStorage): IUserContext
{
    public UserRole? Role { get; private set; }
    public string? CurrentBank { get; private set; }
    public string? CurrentEnterprise { get; private set; }

    public void InitializeRole(UserRole role)
    {
        Role = role;
    }
    
    public void InitializeBank(string bank)
    {
        CurrentBank = bank;
    }

    public void InitializeEnterprise(string enterprise)
    {
        CurrentEnterprise = enterprise;
    }

    public void Clear()
    {
        CurrentBank = null;
        Role = null;
        CurrentEnterprise = null;
    }
    
    public bool IsAuthenticated => ValidateToken();
    public Result<Guid> UserAccountId => GetUserIdFromToken();

    private bool ValidateToken()
    {
        var token = tokenStorage.GetToken();
        return !string.IsNullOrEmpty(token) 
               && tokenService.ValidateToken(token);
    }

    private Result<Guid> GetUserIdFromToken()
    {
        var token = tokenStorage.GetToken();
        return tokenService.GetAccountIdFromToken(token);
    }
}