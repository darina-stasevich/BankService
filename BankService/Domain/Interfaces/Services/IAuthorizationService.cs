using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services;

public interface IAuthorizationService
{
    public Result<UserRole> Authorize(string? login, string? password, string bankName);
    public void Logout();
}