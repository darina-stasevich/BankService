using BankService.Domain.Enums;
using BankService.Domain.Interfaces;

namespace BankService.Application.UserInterationStrategies;

public abstract class BaseMenuStrategy : IMenuStrategy
{
    public abstract void ShowMenu();
    public abstract void HandleInput(int choice);
    
    protected int ReadInt()
    {
        while (true)
        {
            var input = Console.ReadLine();
            var tryParse = int.TryParse(input, out var result);
            if (tryParse)
                return result;
            Console.WriteLine("Invalid input");
        }
    }

    protected decimal ReadDecimal()
    {
        while (true)
        {
            var input = Console.ReadLine();
            var tryParse = decimal.TryParse(input, out var result);
            if (tryParse)
                return result;
            Console.WriteLine("Invalid input");
        }
    }
    
    protected string? GetString(string message, bool canBeEmpty)
    {
        if (!canBeEmpty)
        {
            while (true)
            {
                
                Console.WriteLine($"Enter {message}");
                var input = Console.ReadLine();
                if (input is null)
                    Console.WriteLine("Input cannot be empty");
                else
                    return input;
            }
        }
        Console.WriteLine($"Enter {message}");
        return Console.ReadLine();
    }

    protected Guid GetGuid()
    {
        while (true)
        {
            var input = Console.ReadLine();
            var tryParse = Guid.TryParse(input, out var result);
            if (tryParse)
                return result;
            Console.WriteLine("Invalid input");
        }
    }
    
    protected VerificationStatus GetVerificationStatus()
    {
        while (true)
        {
            var status = GetString("verification status", false)!;
            var parseResult = Enum.TryParse(status, out VerificationStatus receivedVerificationStatus);
            if (parseResult) return receivedVerificationStatus;
        }
    }

    protected BankAccountStatus GetBankAccountStatus()
    {
        while (true)
        {
            var status = GetString("bank account status", false)!;
            var parseResult = Enum.TryParse(status, out BankAccountStatus receivedVerificationStatus);
            if (parseResult) return receivedVerificationStatus;
        }
    }

    protected SalaryProjectRequestStatus GetSalaryProjectRequestStatus()
    {
        while (true)
        {
            var status = GetString("salary project request status", false)!;
            var parseResult = Enum.TryParse(status, out SalaryProjectRequestStatus receivedSalaryProjectRequestStatus);
            if(parseResult) return receivedSalaryProjectRequestStatus;
        }
    }
}