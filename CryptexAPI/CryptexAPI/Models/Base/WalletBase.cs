using CryptexAPI.Enums;
using CryptexAPI.Models.Wallets;
using System.ComponentModel.DataAnnotations;

namespace CryptexAPI.Models.Base;

public abstract class WalletBase : BaseEntity
{
    [MaxLength(12)]
    public SeedPhrase SeedPhrase { get; set; }
    public int SeedPhraseId { get; set; }
    public List<Coin> AmountOfCoins { get; set; }
    public WalletType WalletType { get; set; }

    public void SeedPhraseSet(SeedPhrase seedPhrase)
    {
        SeedPhrase = seedPhrase;
    }
}
