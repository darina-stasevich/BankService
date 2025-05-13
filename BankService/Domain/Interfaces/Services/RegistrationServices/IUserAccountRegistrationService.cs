using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.RegistrationServices;

public interface IUserAccountRegistrationService
{
    public Result<Guid> Register(User user, string bankName, UserRole role, string? enterpriseName = null);
}