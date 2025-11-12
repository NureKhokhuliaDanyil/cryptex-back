using CryptexAPI.Enums;

namespace CryptexAPI.Services.Interfaces;

public interface IBinanceRequestService
{
    Task<double> GetCoinPriceFromBinance(NameOfCoin coinName);
}
