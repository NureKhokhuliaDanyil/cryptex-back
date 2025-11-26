using CryptexAPI.Enums;
using System.Security.Cryptography;
using System.Text;

namespace CryptexAPI.Helpers;

public static class AddressGenerator
{
    private static readonly Random Random = new Random();

    public static string GenerateCryptoAddress(NameOfCoin coin)
    {
        return coin switch
        {
            NameOfCoin.BTC => GenerateBtcAddress(),
            NameOfCoin.XRP => GenerateXrpAddress(),
            _ => GenerateEthAddress()
        };
    }

    private static string GenerateEthAddress()
    {
        var bytes = new byte[20];
        RandomNumberGenerator.Fill(bytes);

        var hex = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        return "0x" + hex;
    }

    private static string GenerateBtcAddress()
    {
        const string chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        var prefix = Random.Next(2) == 0 ? '1' : '3';

        var length = 34;
        var stringBuilder = new StringBuilder(length);
        stringBuilder.Append(prefix);

        for (int i = 1; i < length; i++)
        {
            stringBuilder.Append(chars[Random.Next(chars.Length)]);
        }

        return stringBuilder.ToString();
    }

    private static string GenerateXrpAddress()
    {
        const string chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        var length = 34;
        var stringBuilder = new StringBuilder(length);
        stringBuilder.Append('r');

        for (int i = 1; i < length; i++)
        {
            stringBuilder.Append(chars[Random.Next(chars.Length)]);
        }

        return stringBuilder.ToString();
    }
}