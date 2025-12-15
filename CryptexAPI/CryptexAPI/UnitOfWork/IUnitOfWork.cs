using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.UnitOfWork;

public interface IUnitOfWork
{
    ICoinRepository CoinRepository { get; }
    ISeedPhraseRepository SeedPhraseRepository { get; }
    IUserRepository UserRepository { get; }
    IWalletForMarketRepository WalletForMarketRepository { get; }
    IWalletRepository WalletRepository { get; }
    IFuethersDealRepository FuethersDealRepository { get; }
    ITransactionHistoryRepository TransactionHistoryRepository { get; }
    IMessageRepository MessageRepository { get; }
    ISupportRepository SupportRepository { get; }
    ITicketRepository TicketRepository { get; }
    Task SaveChangesAsync();
}
