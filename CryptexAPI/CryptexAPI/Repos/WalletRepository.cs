using CryptexAPI.Context;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class WalletRepository(AppDbContext context) 
    : BaseRepository<Wallet>(context), IWalletRepository;
