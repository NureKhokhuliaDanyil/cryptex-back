using CryptexAPI.Enums;
using CryptexAPI.Helpers.Constants;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;

    public WalletService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Wallet CreateWallet()
    {
        try
        {
            var wallet = new Wallet();
            wallet.AmountOfCoins = CreateListOfCoins(wallet.Id);
            wallet.SeedPhraseSet(CreateSeedPhrase());

            return wallet;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create wallet {ex.Message}");
        }
    }

    private List<Coin> CreateListOfCoins(int walletId)
    {
        var coinList = new List<Coin>();
        foreach (NameOfCoin name in Enum.GetValues(typeof(NameOfCoin)))
        {
            var coin = new Coin()
            {
                Amount = 0,
                Name = name,
                Price = 0,
                WalletId = walletId
            };
            coinList.Add(coin);
        }
        return coinList;
    }

    private SeedPhrase CreateSeedPhrase()
    {
        try
        {
            var words = new List<string>
            {
                "umbrella", "window", "elephant", "chair", "spaghetti", "notebook", "clover", "ocean",
                "aardvark", "chocolate", "eyebrow", "pigeon", "cup", "rose", "dragon", "cell", "fork",
                "bicycle", "lipstick", "corn", "cow", "flamingo", "ghost", "muffin", "paw", "windmill",
                "potato", "rainbow", "swamp", "whisk", "gnome", "spaceship", "wallet", "dinosaur",
                "elbow", "fiddle", "gorilla", "harp", "igloo", "jackal", "kiwi", "llama", "mango",
                "nugget", "octopus", "peanut", "quokka", "raccoon", "snail", "taco", "unicorn",
                "vampire", "wombat", "xylophone", "yak", "zebra", "atom", "banjo", "cactus", "dolphin",
                "echo", "flannel", "goblin", "hamburger", "iceberg", "jigsaw", "kaleidoscope", "lemon"
            };

            var random = new Random();
            var seedPhrase = new SeedPhrase
            {
                SeedPhraseValues = new List<string>()
            };

            for (var i = 0; i < GlobalConsts.SeedPhraseLength; i++)
            {
                var randomWord = words[random.Next(words.Count)];
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
