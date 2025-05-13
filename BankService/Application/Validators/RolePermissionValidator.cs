using BankService.Domain.Enums;

namespace BankService.Application.Validators;

public class RolePermissionValidator
{
    public static bool CanApproveStatus(UserRole approverRole, BankAccountStatus oldStatus, BankAccountStatus newStatus)
    {
        return approverRole switch
        {
            UserRole.Manager => true,
            UserRole.Operator => true,
            UserRole.Administrator => true,
            UserRole.Client => oldStatus == BankAccountStatus.Active &&
                               (newStatus == BankAccountStatus.Freezed || newStatus == BankAccountStatus.Blocked),

            _ => false
        };
    }

    public static bool CanApproveAccount(UserRole approverRole)
    {
        return approverRole switch
        {
            UserRole.Manager => true,
            UserRole.Operator => true,
            UserRole.Administrator => true,
            _ => false
        };
    }

    public static bool CanApproveLoan(UserRole approverRole)
    {
        return approverRole switch
        {
            UserRole.Manager => true,
            UserRole.Administrator => true,
            _ => false
        };
    }
}