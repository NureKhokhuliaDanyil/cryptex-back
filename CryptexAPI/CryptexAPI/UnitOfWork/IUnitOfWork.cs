using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.UnitOfWork;

public interface IUnitOfWork
{
    ICoinRepository CoinRepository { get; }
    ISeedPhraseRepository SeedPhraseRepository { get; }
    IUserRepository UserRepository { get; }
    IWalletForMarketRepository WalletForMarketRepository { get; }
    IWalletRepository WalletRepository { get; }
    Task SaveChangesAsync();
}
