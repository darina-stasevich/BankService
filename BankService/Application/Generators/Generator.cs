using System.Text;
using BankService.Domain.Interfaces;

namespace BankService.Application.Generators;

public class Generator : IGenerator
{
    private const int PasswordLenght = 10;

    private static readonly int[][] Alphabet =
    {
        new[] { 48, 57 },
        new[] { 65, 90 },
        new[] { 97, 122 }
    };
    
    private readonly Random random = new();

    public string Generate()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < PasswordLenght; i++)
        {
            var randomLine = random.Next(Alphabet.Length);
            var randomChar = (char)random.Next(Alphabet[randomLine][0], Alphabet[randomLine][1]);
            sb.Append(randomChar);
        }

        return sb.ToString();
    }
}