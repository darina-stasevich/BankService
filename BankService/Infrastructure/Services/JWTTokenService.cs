using System.IdentityModel.Tokens.Jwt;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BankService.Domain.Entities;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces;
using BankService.Domain.Results;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BankService.Infrastructure.Services;

public class JwtTokenService(
    string secret,
    string issuer) : ITokenService
{
    //   private readonly ILogger<JWTTokenService> _logger;

 //     ILogger<JWTTokenService> logger)
    //      _logger = logger;

    public string GenerateToken(UserAccount userAccount)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
                new Claim(ClaimTypes.Name, userAccount.Login),
                new Claim(ClaimTypes.Role, userAccount.UserRole.ToString())
            }),
//            Expires = DateTime.UtcNow.AddSeconds(15),
//            Expires = DateTime.UtcNow.AddHours(2),
            Expires = DateTime.UtcNow.AddMinutes(5),   
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);
            
            return true;
        }
        catch (SecurityTokenExpiredException ex)
        {
         //   _logger.LogWarning($"Token expired: {ex.Message}");
            return false;
        }
        catch (SecurityTokenValidationException ex)
        {
     //       _logger.LogWarning($"Token validation failed: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
     //       _logger.LogError(ex, "Unexpected token validation error");
            return false;
        }
    }

    public Result<Guid> GetAccountIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var accountIdClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "nameid")?.Value;

            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new InvalidOperationException("Invalid account ID in token");
            }

            return new Guid(accountIdClaim);
        }
        catch (Exception ex)
        {
     //       _logger.LogError(ex, "Failed to extract account ID from token");
            return Error.Failure(401, ex.Message);
        }
    }

    public Result<UserRole> GetUserRoleFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var userRoleClaim = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "role")?.Value;

            if (string.IsNullOrEmpty(userRoleClaim))
            {
                throw new InvalidOperationException("Invalid account ID in token");
            }

            var t = Enum.Parse(typeof(UserRole), userRoleClaim);
            return (UserRole)t;
        }
        catch (Exception ex)
        {
            //       _logger.LogError(ex, "Failed to extract account ID from token");
            return Error.Failure(400, ex.Message);
        }
    }
}
