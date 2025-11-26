using CryptexAPI.Models;
using CryptexAPI.Models.Persons;
using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Services.Interfaces;

public interface IUserService : ISpotOperations, IAuth
{
    Task<double> GetTotalWalletBalance(int id);
    Task<Wallet> GetMyWallet(int userId);
    Task<User> ChangeBalance(int userId, double amount);
    Task<User> DepositFundsAsync(int userId, double amount);
    Task WithdrawFundsAsync(int userId, double amount);
    Task<List<TransactionHistory>> GetTransactionHistoryAsync(int userId);
    Task<User> DepositCryptoAsync(int userId, string depositAddress, double amount);
}
