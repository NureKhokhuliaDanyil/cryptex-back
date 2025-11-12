using CryptexAPI.Enums;
using CryptexAPI.Helpers;
using CryptexAPI.Services.Interfaces;
using System.Text.Json;

namespace CryptexAPI.Services;

public class BinanceRequestService : IBinanceRequestService
{
    public async Task<double> GetCoinPriceFromBinance(NameOfCoin coinName)
    {
        using var httpClient = new HttpClient();
        var symbol = CoinNameParser.GetBinanceSymbol(coinName);
        var url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";

        var response = await httpClient.GetStringAsync(url);
        var result = JsonSerializer.Deserialize<Dictionary<string, string>>(response);

        if (result != null && 
            result.TryGetValue("price", out var priceStr) &&
            double.TryParse(priceStr, out var price))
        {
            return price;
        }

        return 0;
    }
}
