using CryptexAPI.Enums;
using CryptexAPI.Helpers.Constants;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISeedPhraseService _seedPhraseService;
    private readonly IBinanceRequestService _binanceRequestService;
    public WalletService(IUnitOfWork unitOfWork, ISeedPhraseService seedPhraseService, IBinanceRequestService binanceRequestService)
    {
        _unitOfWork = unitOfWork;
        _seedPhraseService = seedPhraseService;
        _binanceRequestService = binanceRequestService;
    }
    public async Task<Wallet> CreateWallet()
    {
        try
        {
            var wallet = new Wallet();
            wallet.AmountOfCoins = await CreateListOfCoins(wallet.Id);
            wallet.SeedPhraseSet(await CreateSeedPhrase());
            if (wallet == null)
            {
                throw new Exception("Failed to create wallet");
            }

            return wallet;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create wallet {ex.Message}");
        }
    }
    private async Task<List<Coin>> CreateListOfCoins(int walletId)
    {
        var coinList = new List<Coin>();
        foreach (NameOfCoin name in Enum.GetValues(typeof(NameOfCoin)))
        {
            var coin = new Coin() { Amount = 0, Name = name, Price = await _binanceRequestService.GetCoinPriceFromBinance(name), WalletId = walletId };
            coinList.Add(coin);
        }
        return coinList;
    }
    private async Task<SeedPhrase> CreateSeedPhrase()
    {
        try
        {
            var seedPhraseBase = await _seedPhraseService.GetSeedPhraseBase();
            var random = new Random();
            var seedPhrase = new SeedPhrase
            {
                SeedPhraseValues = []
            };

            for (var i = 0; i < GlobalConsts.SeedPhraseLength; i++)
            {
                var randomWord = seedPhraseBase.SeedPhraseValues[random.Next(seedPhraseBase.SeedPhraseValues.Count)];
                seedPhrase.SeedPhraseValues.Add(randomWord);
            }
            return seedPhrase;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create seed phrase {ex.Message}");
        }
    }

    public async Task<Wallet> GetWallet(int walletId)
    {
        try
        {
            var wallet = await _unitOfWork.WalletRepository
                .GetSingleByConditionAsync(e => e.Id == walletId);
            var seedPhrase = await _unitOfWork.SeedPhraseRepository.GetSingleByConditionAsync(e => e.Id == wallet.Data.SeedPhraseId);
            wallet.Data.SeedPhrase = seedPhrase.Data;
            var coins = await _unitOfWork.CoinRepository.GetListByConditionAsync(e => e.WalletId == walletId);
            wallet.Data.AmountOfCoins = (List<Coin>)coins.Data;

            return wallet.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task UpdateCoin(Coin coin)
    {
        try
        {
            await _unitOfWork.CoinRepository.UpdateAsync(coin, e => e.Id == coin.Id);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
