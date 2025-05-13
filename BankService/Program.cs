using BankService.Application;
using BankService.Application.ApprovalStrategy;
using BankService.Application.Generators;
using BankService.Application.Services;
using BankService.Application.UserInterationStrategies;
using BankService.Application.Validators;
using BankService.Domain.Interfaces;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.IValidators;
using BankService.Domain.Interfaces.Services;
using BankService.Domain.Interfaces.Services.ApprovalServices;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Interfaces.Services.TransactionServices;
using BankService.Infrastructure;
using BankService.Infrastructure.Database;
using BankService.Infrastructure.Repositories;
using BankService.Infrastructure.Services;
using BankService.Presentation;
using BankService.Presentation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "../../../../")
    .AddJsonFile("appsettings.json").Build();

var services = new ServiceCollection();

// repositories
services.AddScoped<IBankAccountRepository, BankAccountRepository>();
services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
services.AddScoped<ILoanRepository, LoanRepository>();
services.AddScoped<ILoanRequestRepository, LoanRequestRepository>();
services.AddScoped<ISalaryProjectRepository, SalaryProjectRepository>();
services.AddScoped<ISalaryProjectRequestRepository, SalaryProjectRequestRepository>();
services.AddScoped<IUserAccountRepository, UserAccountRepository>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ITransactionRepository, TransactionRepository>();

services.AddScoped<IGenerator, Generator>();
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
services.AddScoped<ITokenService, JwtTokenService>();

// JWT Token Service с конфигурацией
var jwtSettings = configuration.GetSection("JwtSettings");
services.AddScoped<ITokenService>(_ =>
    new JwtTokenService(
        jwtSettings["Secret"]!,
        jwtSettings["Issuer"]!
    )
);

// validators

services.AddScoped<IAccountCreationDtoValidator, AccountCreationDtoValidator>();
services.AddScoped<IBankAccountValidator, BankAccountValidator>();
services.AddScoped<ICanHasBankAccountValidator, CanHasBankAccountValidator>();
services.AddScoped<ITransferAccountStatusValidator, TransferAccountStatusValidator>();
services.AddScoped<ITransferValidator, TransferValidator>();
services.AddScoped<IUserValidator, UserValidator>();
services.AddScoped<IWithdrawAccountValidator, WithdrawAccountValidator>();
services.AddScoped<IWithdrawValidator, WithdrawValidator>();
services.AddScoped<ILoanRequestDtoValidator, LoanRequestDtoValidator>();

// services

services.AddScoped<IBankAccountApprovalService, BankAccountApprovalService>();
services.AddScoped<ILoanApprovalService, LoanApprovalService>();
services.AddScoped<ISalaryProjectApprovalService, SalaryProjectApprovalService>();
services.AddScoped<IUserAccountApprovalService, UserAccountApprovalService>();
services.AddScoped<IInfoService, InfoService>();
services.AddScoped<IBankAccountRegistrationService, BankAccountRegistrationService>();
services.AddScoped<ILoanRegistrationService, LoanRegistrationService>();
services.AddScoped<ISalaryProjectRegistrationService, SalaryProjectRegistrationService>();
services.AddScoped<IUserAccountRegistrationService, UserAccountRegistrationService>();

services.AddScoped<ITransferService, TransferService>();

// changed
services.AddScoped<IAuthorizationService, AuthorizationService>();

// user context
services.AddSingleton<UserContext>();
services.AddSingleton<IUserContext>(sp => sp.GetRequiredService<UserContext>());

// strategies
services.AddScoped<OperatorApprovalStrategy>();
services.AddScoped<SpecialistApprovalStrategy>();
services.AddScoped<IApprovalStrategy>(provider =>
{
    var factory = provider.GetRequiredService<IStrategyFactory>();
    var context = provider.GetRequiredService<IUserContext>();
    return factory.CreateStrategy(context.Role.Value);
});

// fabrics
services.AddScoped<IAccountFactory, AccountFactory>();
services.AddScoped<IStrategyFactory, StrategyFactory>();

// builder
services.AddScoped<IAccountBuilder, AccountBuilder>();

// application context


services.AddDbContext<ApplicationContext>(options => 
        options.UseSqlite(configuration.GetConnectionString("DefaultConnection")), 
    contextLifetime: ServiceLifetime.Singleton);


services.AddScoped<IMenuStrategyFactory, MenuStrategyFactory>();

services.AddScoped<MainMenuStrategy>();
services.AddScoped<ClientMenuStrategy>();
services.AddScoped<OperatorMenuStrategy>();
services.AddScoped<ManagerMenuStrategy>();
services.AddScoped<AdministratorMenuStrategy>();
services.AddScoped<SpecialistMenuStrategy>();

// token tools
services.AddSingleton<ITokenStorage, MemoryTokenStorage>();

var serviceProvider = services.BuildServiceProvider();

var userContext = serviceProvider.GetRequiredService<UserContext>();
var menuFactory = serviceProvider.GetRequiredService<IMenuStrategyFactory>();
var currentInteractionStrategy = menuFactory.CreateMenuStrategy();
while (true)
    
{
    currentInteractionStrategy.ShowMenu();
    var choiceString = Console.ReadLine();
    var result = Int32.TryParse(choiceString, out int choice);
    if (result == false)
    {
        Console.WriteLine("Please enter a valid choice");
    }
    else
    {
        currentInteractionStrategy.HandleInput(choice);

        currentInteractionStrategy = menuFactory.CreateMenuStrategy(userContext.Role);
        
    }
}
