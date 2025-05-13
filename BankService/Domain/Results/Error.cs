using BankService.Domain.Enums;

namespace BankService.Domain.Results;

public class Error
{
    private Error(
        int code,
        string description,
        ErrorType errorType
    )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public int Code { get; }
    public string Description { get; }
    public ErrorType ErrorType { get; }

    public static Error Failure(int code, string description)
    {
        return new Error(code, description, ErrorType.Failure);
    }

    public static Error NotFound(int code, string description)
    {
        return new Error(code, description, ErrorType.NotFound);
    }

    public static Error Validation(int code, string description)
    {
        return new Error(code, description, ErrorType.Validation);
    }

    public static Error Conflict(int code, string description)
    {
        return new Error(code, description, ErrorType.Conflict);
    }

    public static Error AccessUnAuthorized(int code, string description)
    {
        return new Error(code, description, ErrorType.AccessUnAuthorized);
    }

    public static Error AccessForbidden(int code, string description)
    {
        return new Error(code, description, ErrorType.AccessForbidden);
    }
}