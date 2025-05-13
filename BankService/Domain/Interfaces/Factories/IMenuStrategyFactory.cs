using BankService.Domain.Enums;

namespace BankService.Domain.Interfaces;

public interface IMenuStrategyFactory
{
    public IMenuStrategy CreateMenuStrategy(UserRole? userRole = null);
}