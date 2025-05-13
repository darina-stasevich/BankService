using BankService.Application.ApprovalStrategy;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BankService.Infrastructure;

public class StrategyFactory(IServiceProvider serviceProvider) : IStrategyFactory
{
    public IApprovalStrategy CreateStrategy(UserRole? role)
    {
        using var scope = serviceProvider.CreateScope();
        return role switch
        {
            UserRole.Operator => scope.ServiceProvider.GetRequiredService<OperatorApprovalStrategy>(),
            UserRole.Manager => scope.ServiceProvider.GetRequiredService<OperatorApprovalStrategy>(),
            UserRole.Administrator => scope.ServiceProvider.GetRequiredService<OperatorApprovalStrategy>(),
            UserRole.ExternalSpecialist => scope.ServiceProvider.GetRequiredService<SpecialistApprovalStrategy>(),
            _ => throw new ArgumentException("Unsupported role")
        };
    }
}