using CryptexAPI.Enums;
using CryptexAPI.Models.Base;

namespace CryptexAPI.Models.Wallets;

public class WalletForMarket : WalletBase
{
    public WalletForMarket()
    {
        Id = 1;
        WalletType = WalletType.Market;
    }
}
