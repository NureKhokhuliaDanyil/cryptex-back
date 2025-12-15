using CryptexAPI.Enums;
using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Services.Interfaces;

public interface ICoinService
{
    Task<Coin> UpdatePrice(int id);
    Task<List<PriceHistoryItem>> GetPriceHistory(NameOfCoin coin, BinanceInterval timePeriod);
}
