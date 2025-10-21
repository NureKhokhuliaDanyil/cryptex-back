using CryptexAPI.Context;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class WalletForMarketRepository(AppDbContext context)
    : BaseRepository<WalletForMarket>(context), IWalletForMarketRepository;
