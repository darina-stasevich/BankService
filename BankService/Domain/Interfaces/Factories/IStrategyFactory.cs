using BankService.Domain.Enums;

namespace BankService.Domain.Interfaces;

public interface IStrategyFactory
{
    IApprovalStrategy CreateStrategy(UserRole? role);
}
