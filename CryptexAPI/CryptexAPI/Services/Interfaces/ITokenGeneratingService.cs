using CryptexAPI.Models.Persons;
using System.Security.Claims;

namespace CryptexAPI.Services.Interfaces;

public interface ITokenGeneratingService
{
    string GenerateToken(User user);
    string GenerateToken(List<Claim> userClaims);
}
