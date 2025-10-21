using CryptexAPI.Models.Base;

namespace CryptexAPI.Models.Wallets;

public class Wallet : WalletBase
{
    public Wallet()
    {
        WalletType = Enums.WalletType.User;
    }
}
