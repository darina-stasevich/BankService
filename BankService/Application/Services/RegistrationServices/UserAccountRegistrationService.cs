using BankService.Application.Validators;
using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.IValidators;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class UserAccountRegistrationService(
    IEnterpriseRepository bankRepository,
    IUserRepository userRepository,
    IUserAccountRepository userAccountRepository,
    IEnterpriseRepository enterpriseRepository,
    IUserValidator userValidator) : IUserAccountRegistrationService
{
  
    public Result<Guid> Register(User user, string bankName, UserRole role, string? enterpriseName = null)
    {
        var enterprise = enterpriseRepository.GetByName(enterpriseName);
        if (role == UserRole.ExternalSpecialist)
        {
            if(enterpriseName == null)
                return Error.Validation(400, $"Enterprise not specified for specialist");
            if(enterprise == null)
                return Error.NotFound(400, $"Enterprise with name {enterpriseName} not found");
        }
        
        if (bankRepository.IsBank(bankName) == false) 
            return Error.NotFound(400, $"Bank with name {bankName} does not exist.");

        var bank = enterpriseRepository.GetByName(bankName);
        if (userRepository.FindBySameData(user) != null)
            // user already registered
            return HandleExistingUser(user, bank!.Id, role, enterprise!=null?enterprise.Id:null);

        if (userRepository.FindByUniqueData(user) != null)
            // someone already has this unique information
            return Error.Conflict(400, $"User {user.Id} has duplicate data with other user.");

        // user is not in base yet
        var result = userValidator.Validate(user);
        if (!result.IsValid)
            // invalid data
            // can create create_account request
            return Error.Validation(400, $"User {user.LastName} {user.FirstName} send invalid data.");
        else
        {
            user.Status = VerificationStatus.Verified;
            userRepository.Add(user);
        }

        return CreateAccount(user.Id, bank.Id, role, enterprise?.Id);
    }

    private Result<Guid> HandleExistingUser(User user, Guid bankId, UserRole role, Guid? enterpriseId = null)
    {
        user = userRepository.FindBySameData(user);
        if (user!.Status == VerificationStatus.Rejected)
            return Error.Conflict(400, $"User {user.LastName} {user.FirstName} data is rejected.");
        return CreateAccount(user.Id, bankId, role, enterpriseId);
    }

    private Result<Guid> CreateAccount(Guid userId, Guid bankId, UserRole role, Guid? enterpriseId = null)
    {
        
        UserAccount account = new()
        {
            UserId = userId,
            BankId = bankId,
            UserRole = role,
            Status = VerificationStatus.Pending,
            ChangedStatusAt = DateTime.Now
        };
        if(role == UserRole.ExternalSpecialist)
            account.EnterpriseId = enterpriseId;
        userAccountRepository.Add(account);
        return account.Id;
    }
}