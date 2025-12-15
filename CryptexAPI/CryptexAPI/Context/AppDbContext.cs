using CryptexAPI.Models;
using CryptexAPI.Models.Persons;
using CryptexAPI.Models.Wallets;
using Microsoft.EntityFrameworkCore;

namespace CryptexAPI.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Coin> Coins { get; set; }
    public DbSet<SeedPhrase> SeedPhrases { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletForMarket> WalletForMarkets { get; set; }
    public DbSet<FuethersDeal> FuethersDeals { get; set; }
    public DbSet<TransactionHistory> TransactionHistories { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Support> Supports { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
}
