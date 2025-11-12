using CryptexAPI.Models.Base;

namespace CryptexAPI.Models.Wallets;

public class SeedPhrase : BaseEntity
{
    public List<string> SeedPhraseValues { get; set; }
}
