using CryptexAPI.Enums;

namespace CryptexAPI.Services.Interfaces;

public interface ISpotOperations
{
    Task BuyCoin(int userId, NameOfCoin coin, double amount);
    Task SellCoin(int userId, NameOfCoin coin, double amount);
    Task ConvertCurrency(int idOfUser, NameOfCoin CoinForConvert, NameOfCoin imWhichCoinConvert,
        double amountOfCoinForConvert);
}
