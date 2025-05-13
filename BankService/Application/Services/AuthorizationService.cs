using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services;
using BankService.Domain.Results;
using BankService.Infrastructure.Repositories;

namespace BankService.Application.Services;

public class AuthorizationService(
    IUserAccountRepository userAccountRepository,
    IPasswordHasher passwordHasher,
    IEnterpriseRepository bankRepository,
    ITokenService tokenService,
    ITokenStorage tokenStorage
) : IAuthorizationService
{
    public Result<UserRole> Authorize(string? login, string? password, string bankName)
    {
        if (password == null || login == null)
            return Error.Failure(400, "login and password must be not empty.");

        if(!bankRepository.IsBank(bankName))
            return Error.Validation(400, "bank id must be valid.");
        var bank = bankRepository.GetByName(bankName);
        var account = userAccountRepository.GetByLogin(login, bank.Id);
        if (account == null)
            return Error.NotFound(400, $"User with login {login} not found.");
        
        if (!passwordHasher.VerifyHashedPassword(account.Password, password))
            return Error.Failure(401, "You entered incorrect password.");

        var token = tokenService.GenerateToken(account);
        tokenStorage.SaveToken(token);
        return account.UserRole;
    }

    public void Logout()
    {
        tokenStorage.Clear();
    }
}