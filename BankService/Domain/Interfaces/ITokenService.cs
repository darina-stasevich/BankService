using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserAccount userAccount);
    bool ValidateToken(string token);
    Result<Guid> GetAccountIdFromToken(string token);
    
}