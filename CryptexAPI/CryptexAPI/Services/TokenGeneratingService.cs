using CryptexAPI.Models.Identity;
using CryptexAPI.Models.Persons;
using CryptexAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CryptexAPI.Services;

public class TokenGeneratingService : ITokenGeneratingService
{
    public string? GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Name", user.Name ?? ""),
            new Claim("Surname", user.Surname),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(14),
            Audience = SD.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SD.JWTKey)), SecurityAlgorithms.HmacSha256Signature),
            Issuer = SD.Issuer
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        var jwtToken = handler.WriteToken(token);

        return jwtToken;
    }

    public string? GenerateToken(List<Claim> userInfo)
    {
        var claims = userInfo.Where(x => x.Type == ClaimTypes.Email || x.Type == ClaimTypes.Name);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Audience = SD.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SD.JWTKey)),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = SD.Issuer
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        var jwtToken = handler.WriteToken(token);

        return jwtToken;
    }
}
