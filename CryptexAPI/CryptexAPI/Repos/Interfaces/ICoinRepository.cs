using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Repos.Interfaces;

public interface ICoinRepository : IBaseRepository<Coin>
{
    Task UpdatePricesFromBinance();
}
