using CryptexAPI.Enums;
using CryptexAPI.Models.Base;

namespace CryptexAPI.Models.Wallets;

public class Coin : BaseEntity
{
    public NameOfCoin Name { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; } = 0;
    public int WalletId { get; set; }
    public string? DepositAddress { get; set; }
}
