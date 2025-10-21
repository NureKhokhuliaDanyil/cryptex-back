using CryptexAPI.Enums;

namespace CryptexAPI.Helpers;

public static class CoinNameParser
{
    public static string? GetBinanceSymbol(NameOfCoin coin)
    {
        return coin switch
        {
            NameOfCoin.BTC => "BTCUSDT",
            NameOfCoin.ETH => "ETHUSDT",
            NameOfCoin.LTC => "LTCUSDT",
            NameOfCoin.BNB => "BNBUSDT",
            NameOfCoin.SOL => "SOLUSDT",
            NameOfCoin.XRP => "XRPUSDT",
            _ => null
        };
    }
}
