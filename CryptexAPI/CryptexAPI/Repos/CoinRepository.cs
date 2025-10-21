using CryptexAPI.Context;
using CryptexAPI.Enums;
using CryptexAPI.Helpers;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace CryptexAPI.Repos;

public class CoinRepository(AppDbContext context) : BaseRepository<Coin>(context), ICoinRepository
{
    public async Task UpdatePricesFromBinance()
    {
        using var httpClient = new HttpClient();
        var prices = new Dictionary<NameOfCoin, double>();
        foreach (NameOfCoin coin in Enum.GetValues(typeof(NameOfCoin)))
        {
            var symbol = CoinNameParser.GetBinanceSymbol(coin);

            try
            {
                var url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";
                var response = await httpClient.GetStringAsync(url);

                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
                if (result != null)
                {
                    prices[coin] = double.Parse(result["price"], CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                prices[coin] = 0;
            }
        }

        var coins = await context.Coins.ToListAsync();
        foreach (var coin in coins)
        {
            if (prices.TryGetValue(coin.Name, out var price))
            {
                coin.Price = price;
                context.Coins.Update(coin);
            }
        }
        await context.SaveChangesAsync();
    }
}
