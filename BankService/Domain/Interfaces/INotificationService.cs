using FluentValidation.Results;

namespace BankService.Domain.Interfaces;

public interface INotificationService
{
    void SendValidationFailedNotification(string receiver, List<ValidationFailure> validationFailures);

    public void SendNotification(string receiver, string info, List<string> data);
    void SendCredentials(string receiver, string login, string password);
}