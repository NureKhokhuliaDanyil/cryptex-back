using CryptexAPI.Enums;
using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Models;

public class FuethersDealRequest
{
    public Coin Coin { get; set; }
    public TypeOfFuetersDeal TypeOfDeal { get; set; }
    public int Leverage { get; set; }
    public int UserId { get; set; }
    public double StopLoss { get; set; }
    public double TakeProfit { get; set; }
    public double MarginValue { get; set; }
    public double Amount { get; set; }
}
