namespace BankService.Domain.Interfaces;

public interface ITokenStorage
{
    public void SaveToken(string token);
    public string GetToken();
    public void Clear();
}