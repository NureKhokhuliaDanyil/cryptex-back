using CryptexAPI.Enums;
using CryptexAPI.Models.Base;
using CryptexAPI.Models.Persons;

namespace CryptexAPI.Models;

public class TransactionHistory : BaseEntity
{
    public int UserId { get; set; }

    public User User { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public TransactionType Type { get; set; } 

    public NameOfCoin? CoinName { get; set; }

    public double CoinAmount { get; set; }

    public double PricePerCoin { get; set; }

    public double UsdValueChange { get; set; }

    public string? Notes { get; set; }
}
