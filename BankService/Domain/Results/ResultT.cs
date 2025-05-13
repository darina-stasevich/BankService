namespace BankService.Domain.Results;

public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
    {
        _value = value;
    }

    private Result(Error error) : base(error)
    {
        _value = default;
    }

    public T Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value for failed result");

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return new Result<T>(error);
    }
}