using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BankService.Application.UserInterationStrategies;

public class MenuStrategyFactory(IServiceProvider serviceProvider) : IMenuStrategyFactory
{
    public IMenuStrategy CreateMenuStrategy(UserRole? userRole = null)
    {
        return userRole switch
        {
            UserRole.ExternalSpecialist => serviceProvider.GetRequiredService<SpecialistMenuStrategy>(),
            UserRole.Client => serviceProvider.GetRequiredService<ClientMenuStrategy>(),
            UserRole.Operator => serviceProvider.GetRequiredService<OperatorMenuStrategy>(),
            UserRole.Manager => serviceProvider.GetRequiredService<ManagerMenuStrategy>(),
            UserRole.Administrator => serviceProvider.GetRequiredService<AdministratorMenuStrategy>(),
            _ => serviceProvider.GetRequiredService<MainMenuStrategy>()
        };

    }
}