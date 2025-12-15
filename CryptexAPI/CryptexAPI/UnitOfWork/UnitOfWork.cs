using CryptexAPI.Context;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _dbContext;
    private bool disposed = false;
    public ICoinRepository CoinRepository { get; private set; }
    public ISeedPhraseRepository SeedPhraseRepository { get; private set; }
    public IUserRepository UserRepository { get; private set; }
    public IWalletForMarketRepository WalletForMarketRepository { get; private set; }
    public IWalletRepository WalletRepository { get; private set; }
    public IFuethersDealRepository FuethersDealRepository { get; private set; }
    public ITransactionHistoryRepository TransactionHistoryRepository { get; private set; }
    public IMessageRepository MessageRepository { get; private set; }
    public ISupportRepository SupportRepository { get; private set; }
    public ITicketRepository TicketRepository { get; private set; }

    public UnitOfWork(AppDbContext dbContext,
        ICoinRepository coinRepository,
        ISeedPhraseRepository seedPhraseRepository,
        IUserRepository userRepository,
        IWalletForMarketRepository walletForMarketRepository,
        IWalletRepository walletRepository,
        IFuethersDealRepository fuethersDealRepository,
        ITransactionHistoryRepository transactionHistoryRepository, 
        IMessageRepository messageRepository,
        ISupportRepository supportRepository,
        ITicketRepository ticketRepository)
    {
        _dbContext = dbContext;
        CoinRepository = coinRepository;
        SeedPhraseRepository = seedPhraseRepository;
        UserRepository = userRepository;
        WalletForMarketRepository = walletForMarketRepository;
        WalletRepository = walletRepository;
        FuethersDealRepository = fuethersDealRepository;
        TransactionHistoryRepository = transactionHistoryRepository;
        MessageRepository = messageRepository;
        SupportRepository = supportRepository;
        TicketRepository = ticketRepository;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
