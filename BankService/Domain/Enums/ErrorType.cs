namespace BankService.Domain.Enums;

public enum ErrorType
{
    Failure,
    NotFound,
    Validation,
    Conflict,
    AccessUnAuthorized,
    AccessForbidden
}