using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Services.Interfaces;

public interface IWalletService
{
    Wallet CreateWallet();
    Task<Wallet> GetWallet(int walletId);
    Task UpdateCoin(Coin coin);
}
