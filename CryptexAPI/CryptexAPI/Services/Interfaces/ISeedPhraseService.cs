using CryptexAPI.Models.Wallets;

namespace CryptexAPI.Services.Interfaces;

public interface ISeedPhraseService
{
    Task AddBaseWords();
    Task<SeedPhrase> GetSeedPhraseBase();
}
