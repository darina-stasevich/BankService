namespace BankService.Infrastructure.Repositories;

public interface IPasswordHasher
{
    public string HashPassword(string password);

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword);

}