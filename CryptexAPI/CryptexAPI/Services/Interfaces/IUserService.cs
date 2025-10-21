using CryptexAPI.Models.Persons;
using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Services.Interfaces;

public interface IUserService : ISpotOperations, IAuth
{
    Task<double> GetTotalWalletBalance(int id);
    Task<Wallet> GetMyWallet(int userId);
    Task<User> ChangeBalance(int userId, double amount);
}
