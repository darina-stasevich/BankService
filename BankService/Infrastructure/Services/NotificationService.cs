using BankService.Domain.Interfaces;
using FluentValidation.Results;

namespace BankService.Application.Services;

public class NotificationService : INotificationService
{
    public void SendValidationFailedNotification(string email, List<ValidationFailure> validationFailures)
    {
        Console.WriteLine($"Receiver: {email}");
        Console.WriteLine("Your validation failed. Errors are:");
        foreach (var error in validationFailures) Console.WriteLine(error.ErrorMessage);
    }

    public void SendNotification(string email, string info = null, List<string> data = null)
    {
        Console.WriteLine($"Receiver: {email}");
        if (info != null)
            Console.WriteLine(info);
        if (data != null)
            foreach (var unit in data)
                Console.WriteLine(unit);
    }

    public void SendCredentials(string email, string login, string password)
    {
        Console.WriteLine($"Receiver: {email}");
        Console.WriteLine("Registration completed!");
        Console.WriteLine("Your login and password:");
        Console.WriteLine($"Login: {login}");
        Console.WriteLine($"Password: {password}");
    }
}