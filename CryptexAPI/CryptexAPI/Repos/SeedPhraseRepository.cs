using CryptexAPI.Context;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class SeedPhraseRepository(AppDbContext context) 
    : BaseRepository<SeedPhrase>(context), ISeedPhraseRepository;
