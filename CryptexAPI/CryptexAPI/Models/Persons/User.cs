using CryptexAPI.Enums;
using CryptexAPI.Models.Base;
using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Models.Persons;

public class User : ProfileBase
{
    public Wallet Wallet { get; set; }
    public int WalletId { get; set; }
    public User()
    {
        Role = Role.User;
        Balance = 0;
    }

    public User(string name, string surname, string phoneNumber, string address, int age, string email,
        string country, Gender gender, Role role, double? income, int id, Wallet wallet, List<int> followers)
    {
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
        Adress = address;
        Age = age;
        Email = email;
        Country = country;
        Gender = gender;
        Role = role;
        FollowersIds = followers;
        Income = income;
        Id = id;
        Wallet = wallet;
    }
}
