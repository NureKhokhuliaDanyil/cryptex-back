using CryptexAPI.Enums;

namespace CryptexAPI.Models.Persons;

public class Support : User
{
    public int Experience { get; set; } = 0;
    public int Salary { get; set; } = 200;
    public Ticket TicketInProgress { get; set; }
    public int? TicketInProgressId { get; set; }
    public Support() { }
    public Support(User user)
    {
        Role = Role.Support;
        Name = user.Name;
        Surname = user.Surname;
        Email = user.Email;
        Age = user.Age;
        Country = user.Country;
        PhoneNumber = user.PhoneNumber;
        Adress = user.Adress;
        Gender = user.Gender;
        Wallet = user.Wallet;
        WalletId = user.WalletId;
        FollowersIds = user.FollowersIds;
        IsBanned = user.IsBanned;
        Balance = user.Balance;
        BalanceForCopyTrading = user.BalanceForCopyTrading;
        Id = user.Id;
        Income = user.Income;
        TicketInProgressId = null;
    }
    public Support(User user, int experience, int salary)
    {
        Role = Role.Support;
        Name = user.Name;
        Surname = user.Surname;
        Email = user.Email;
        Age = user.Age;
        Country = user.Country;
        PhoneNumber = user.PhoneNumber;
        Gender = user.Gender;
        Wallet = user.Wallet;
        WalletId = user.WalletId;
        Adress = user.Adress;
        FollowersIds = user.FollowersIds;
        IsBanned = user.IsBanned;
        Balance = user.Balance;
        BalanceForCopyTrading = user.BalanceForCopyTrading;
        Id = user.Id;
        Income = user.Income;
        Experience = experience;
        Salary = salary;
        TicketInProgressId = null;
    }
}