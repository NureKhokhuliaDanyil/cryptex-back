using CryptexAPI.Enums;
using System.Data;
using System.Reflection;

namespace CryptexAPI.Models.Base;

public abstract class ProfileBase : BaseEntity
{
    public string Name { get; set; }
    public string? GoogleID { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public string Adress { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public string? Country { get; set; }
    public Gender Gender { get; set; }
    public Role Role { get; set; }
    public List<int>? FollowersIds { get; set; }
    public double? Income { get; set; }
    public double? Balance { get; set; }
    public double? BalanceForCopyTrading { get; set; }
    public bool IsBanned { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public bool IsSilent { get; set; } = true;
}
