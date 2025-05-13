using BankService.Application.Services;
using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using Microsoft.IdentityModel.Abstractions;

namespace BankService.Application.UserInterationStrategies;

public class MainMenuStrategy(
    IUserContext userContext,
    IAuthorizationService authorizationService,
    IUserAccountRegistrationService userAccountRegistrationService,
    IInfoService infoService) : BaseMenuStrategy
{
    public override void ShowMenu()
    {
        Console.WriteLine("Welcome to Bank Service");
        Console.WriteLine("==== Main menu ====");
        Console.WriteLine("Available actions:");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("0. Exit");
    }

    private void GetBank()
    {
        while (userContext.CurrentBank == null)
        {
            Console.WriteLine("Enter the bank to continue");
            Console.WriteLine("Available actions:");
            Console.WriteLine("1. Enter the name of the bank");
            Console.WriteLine("2. Get list of available banks");
            Console.WriteLine("0. Exit");
            var choice = ReadInt();
            if(choice == 0)
                HandleInput(choice);
            else
                HandleInput(100 + choice);
        }
    }
    private void GetEnterprise()
    {
        while (userContext.CurrentEnterprise == null)
        {
            Console.WriteLine("Enter the name of the enterprise");
            Console.WriteLine("Available actions:");
            Console.WriteLine("1. Enter the name of the enterprise");
            Console.WriteLine("2. Get list of available enterprises");
            Console.WriteLine("0. Exit");
            var choice = ReadInt();
            if(choice == 0)
                HandleInput(choice);
            else
                HandleInput(200 + choice);
        }
    }
    private UserRole GetRole()
    {
        while (true)
        {
            Console.WriteLine("Enter role you want.");
            Console.WriteLine("C - for client");
            Console.WriteLine("O - for operator");
            Console.WriteLine("M - for manager");
            Console.WriteLine("A - for administrator");
            Console.WriteLine("S - for external specialist");
            Console.WriteLine("Q - quit");
            var roleString = Console.ReadLine();
            switch (roleString)
            {
                case "C": return UserRole.Client;
                case "O": return UserRole.Operator;
                case "M": return UserRole.Manager;
                case "A": return UserRole.Administrator;
                case "S": return UserRole.ExternalSpecialist;
                case "Q":
                {
                    HandleInput(0);
                    break;
                }
                default:
                {
                    Console.WriteLine("Invalid input");
                    break;
                }
            }
        }
    }
    public override void HandleInput(int choice)
    {
        switch (choice)
        {
            case 1:
            {
                // authorization
                userContext.Clear();
                GetBank();
                var login = GetString("login", false);
                var password = GetString("password", false);    
                
                var authorizationResult = authorizationService.Authorize(login, password, userContext.CurrentBank);
                if (!authorizationResult.IsSuccess)
                {
                    Console.WriteLine("Invalid login or password");
                }
                else
                {
                    Console.WriteLine("Authenticated");
                    Console.WriteLine($"Your role: {authorizationResult.Value.ToString()}");
                    userContext.InitializeRole(authorizationResult.Value);
                    
                    // TODO log
                }
                break;
            }
            case 2:
            {
                // registration
                userContext.Clear();
                GetBank();
                UserRole role = GetRole();
                var lastName = GetString("last name", false);
                var firstName = GetString("first name", false);
                var secondName = GetString("second name if you has", true);
                var email = GetString("email", false);
                bool isResident = false;
                while (!bool.TryParse(GetString("true if you a resident of Belarus, false otherwise", false),
                           out isResident))
                {
                    Console.WriteLine("Invalid input.");
                }
                var phoneNumber = GetString("phone number", false);
                User userDto = new User
                {
                    LastName = lastName!,
                    FirstName = firstName!,
                    SecondName = secondName,
                    Email = email!,
                    IsResident = isResident,
                    PhoneNumber = phoneNumber!
                };
                var passportNumber = GetString("passport number", false);
                var passportId = GetString("passport id", false);
                if (userDto.IsResident)
                {
                    userDto.NationalPassportNumber = passportNumber;
                    userDto.NationalPassportID = passportId;
                }
                else
                {
                    userDto.ForeignPassportNumber = passportNumber;
                    userDto.ForeignPassportID = passportId;
                }

                if (role == UserRole.ExternalSpecialist)
                {
                   GetEnterprise();
                }

                
                var resultRegistration = userAccountRegistrationService.Register(userDto, userContext.CurrentBank, role, userContext.CurrentEnterprise);
                // TODO log
                
                if (resultRegistration.IsSuccess)
                {
                    Console.WriteLine("Registration successful. Your data is pending");
                }
                else
                {
                    Console.WriteLine("Registration failed. Check your data. Be sure there is no mistake");
                    Console.WriteLine(resultRegistration.Error.Description);
                }
                break;
            }

            case 101:
            {
                Console.WriteLine("Enter name of the bank to continue");
                var name = Console.ReadLine();
                userContext.InitializeBank(name);
                break;
            }

            case 102:
            {
                var banks = infoService.GetBanks().ToList();
                Console.WriteLine("Available banks:");
                foreach (var bank in banks)
                    Console.WriteLine($" - {bank}");
                if(banks.Count == 0)
                    Console.WriteLine();
                break;
            }


            case 201:
            {
                Console.WriteLine("Enter name of the enterprise to continue");
                var name = Console.ReadLine();
                userContext.InitializeEnterprise(name);
                break;
            }

            case 202:
            {
                var enterprises = infoService.GetEnterprises().ToList();
                Console.WriteLine("AvailableEnterprises:");
                foreach (var enterprise in enterprises)
                    Console.WriteLine($" - {enterprise}");
                if(enterprises.Count == 0)
                    Console.WriteLine();
                break;
            }
            case 0:
            {
                Environment.Exit(0);
                break;
            }
            default:
            {
                Console.WriteLine("Invalid choice.");
                break;
            }
        }
    }
}
