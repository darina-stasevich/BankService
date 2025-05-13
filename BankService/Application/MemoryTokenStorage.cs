using BankService.Domain.Interfaces;

namespace BankService.Application;

public class MemoryTokenStorage : ITokenStorage 
{
    private string _token;

    public void SaveToken(string token) => _token = token;
    public string GetToken() => _token;
    public void Clear() => _token = null;
}