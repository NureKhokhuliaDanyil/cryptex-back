using CryptexAPI.Enums;
using CryptexAPI.Exceptions;
using CryptexAPI.Helpers;
using CryptexAPI.Models;
using CryptexAPI.Models.Identity;
using CryptexAPI.Models.Persons;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services;

public class UserService : IUserService
{
    private readonly IWalletService _walletService;

    private readonly IUnitOfWork _unitOfWork;

    public UserService(IWalletService walletService, IUnitOfWork unitOfWork)
    {
        _walletService = walletService;
        _unitOfWork = unitOfWork;
    }

    #region IAuth Implementation
    public async Task<User> GetById(int id)
    {
        var user = await _unitOfWork.UserRepository
            .GetSingleByConditionAsync(u => u.Id == id);
        return user.Data;
    }
    public async Task<User> GetByGoogleId(string googleId)
    {
        if (!string.IsNullOrEmpty(googleId))
        {
            var user = await _unitOfWork.UserRepository.GetSingleByConditionAsync(u => u.GoogleID == googleId);
            return user.Data;
        }

        return null;
    }
    public async Task<User> Insert(
        RegistrationModel registrationModel,
        Wallet wallet,
        bool IsGoogleRegistration = false
        )
    {
        if (registrationModel != null)  
        {
            var parsedRole = Enum.Parse<Role>(registrationModel.Role);
            var baseUser = new User()
            {
                GoogleID = registrationModel.GoogleID,
                Email = registrationModel.Email,
                Name = registrationModel.Name,
                Surname = registrationModel.Surname,
                PhoneNumber = registrationModel.PhoneNumber,
                Wallet = wallet,
                WalletId = wallet.Id,
                Age = registrationModel.Age,
                Country = registrationModel.Country,
                Adress = registrationModel.Adress,
                Role = parsedRole
            };

            var userEntity = parsedRole switch
            {
                _ => baseUser
            };

            if (!IsGoogleRegistration)
            {
                var hashedPassword = PasswordHasher.HashPassword(registrationModel.Password);
                userEntity.PasswordHash = hashedPassword.hash;
                userEntity.PasswordSalt = hashedPassword.salt;
            }

            await _unitOfWork.UserRepository.AddAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();

            return userEntity;
        }

        return null;
    }

    public async Task<User> Login(LoginModel loginModel)
    {
        if (loginModel != null
            && !string.IsNullOrEmpty(loginModel.Email)
            && !string.IsNullOrEmpty(loginModel.Password))
        {
            var result = await _unitOfWork.UserRepository
                .GetSingleByConditionAsync(x => x.Email == loginModel.Email);
            var user = result.Data;

            if (user != null && PasswordHasher.VerifyPassword(loginModel.Password, user.PasswordHash, user.PasswordSalt))
            {
                return user;
            }
        }

        return null;
    }

    public async Task<bool> Update(User user)
    {
        await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == user.Id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    #endregion

    #region ISpotOperations Implementation (Transactional)
    public async Task BuyCoin(int id, NameOfCoin coin, double amount)
    {
        try
        {
            var result = await _unitOfWork.UserRepository
                .GetSingleByConditionAsync(e => e.Id == id);

            if (!result.IsSuccess)
            {
                throw new Exception($"Failed to get wallet");
            }

            var user = result.Data;
            user.Wallet = await GetMyWallet(user.Id);
            var coinInWallet = user.Wallet.AmountOfCoins.FirstOrDefault(c => c.Name == coin);

            if (coinInWallet == null)
            {
                throw new Exception($"Coin {coin} not found in user's wallet");
            }

            var moneyForThisOperation = coinInWallet.Price * amount;

            if (moneyForThisOperation > user.Balance)
            {
                throw new Exception("Balance is less than required");
            }

            user.Balance += -moneyForThisOperation;
            coinInWallet.Amount += amount;
            await _walletService.UpdateCoin(coinInWallet);
            await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == id);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"failed to buy Coin {coin}. {e.Message}");
        }
    }
    public async Task SellCoin(int id, NameOfCoin coin, double amount)
    {
        try
        {
            var result = await _unitOfWork.UserRepository
                .GetSingleByConditionAsync(e => e.Id == id);

            if (!result.IsSuccess)
            {
                throw new Exception($"Failed to get wallet");
            }

            var user = result.Data;
            user.Wallet = await GetMyWallet(user.Id);
            var coinInWallet = user.Wallet.AmountOfCoins.FirstOrDefault(c => c.Name == coin);

            if (coinInWallet == null)
            {
                throw new Exception($"Coin {coin} not found in user's wallet");
            }

            if (coinInWallet.Amount < amount)
            {
                throw new Exception($"Amount of coin in wallet is less than you want to sell");

            }
            var moneyIncomeAfterOperation = coinInWallet.Price * amount;
            user.Balance += moneyIncomeAfterOperation;
            coinInWallet.Amount -= amount;
            await _walletService.UpdateCoin(coinInWallet);
            await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == id);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"{e.Message}");
        }
    }

    public async Task ConvertCurrency(
        int idOfUser,
        NameOfCoin CoinForConvert,
        NameOfCoin imWhichCoinConvert,
        double amountOfCoinForConvert
        )
    {
        try
        {
            var result = await _unitOfWork.UserRepository
                .GetSingleByConditionAsync(e => e.Id == idOfUser);
            if (!result.IsSuccess)
            {
                throw new Exception($"Failed to get wallet");
            }
            var user = result.Data;
            var coinForConvert = user.Wallet.AmountOfCoins.FirstOrDefault(e => e.Name == CoinForConvert);
            var coinToConvertInto = user.Wallet.AmountOfCoins.FirstOrDefault(e => e.Name == imWhichCoinConvert);

            if (coinForConvert == null || coinToConvertInto == null)
            {
                throw new Exception("One of the coins was not found in the wallet.");
            }

            if (coinForConvert.Amount < amountOfCoinForConvert)
            {
                throw new Exception("Not enough coins for conversion.");
            }

            var amountAfterConversion = (coinForConvert.Price / coinToConvertInto.Price) * amountOfCoinForConvert;
            coinForConvert.Amount -= amountOfCoinForConvert;
            coinToConvertInto.Amount += amountAfterConversion;
            await _walletService.UpdateCoin(coinForConvert);
            await _walletService.UpdateCoin(coinToConvertInto);
            await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == idOfUser);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Fail to convert coin: {ex.Message}");
        }
    }

    #endregion

    #region Other IUserService Methods
    public async Task<double> GetTotalWalletBalance(int id)
    {
        var user = await GetUserByIdAsync(id);
        return user.Wallet.AmountOfCoins.Sum(coin => coin.Amount * coin.Price);
    }

    public async Task<Wallet> GetMyWallet(int userId)
    {
        var user = await GetUserByIdAsync(userId);
        return user.Wallet;
    }

    public async Task<User> DepositFundsAsync(int userId, double amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be positive.");
        }

        var user = await GetUserByIdAsync(userId);

        user.Balance = (user.Balance ?? 0) + amount;

        var history = CreateHistoryEntry(userId, TransactionType.Deposit, amount, null, amount, 1);
        await _unitOfWork.TransactionHistoryRepository.AddAsync(history);

        await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == userId);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task WithdrawFundsAsync(int userId, double amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be positive.");
        }

        var user = await GetUserByIdAsync(userId);

        if ((user.Balance ?? 0) < amount)
        {
            throw new InvalidOperationException("Insufficient balance to withdraw funds.");
        }

        user.Balance -= amount;

        var history = CreateHistoryEntry(userId, TransactionType.Withdraw, -amount, null, -amount, 1);
        await _unitOfWork.TransactionHistoryRepository.AddAsync(history);

        await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == userId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<TransactionHistory>> GetTransactionHistoryAsync(int userId)
    {
        await GetUserByIdAsync(userId);

        var historyResult = await _unitOfWork.TransactionHistoryRepository
            .GetListByConditionAsync(h => h.UserId == userId);

        if (!historyResult.IsSuccess)
        {
            return new List<TransactionHistory>(); 
        }

        return historyResult.Data.OrderByDescending(h => h.Timestamp).ToList();
    }

    public Task<User> ChangeBalance(int userId, double amount)
    {
        if (amount > 0)
        {
            return DepositFundsAsync(userId, amount);
        }
        else
        {
            throw new ArgumentException("Use WithdrawFundsAsync for negative amounts.");
        }
    }

    #endregion

    #region Private Helpers
    private async Task<User> GetUserByIdAsync(int id)
    {
        var userResult = await _unitOfWork.UserRepository
            .GetSingleByConditionAsync(u => u.Id == id);

        if (!userResult.IsSuccess)
        {
            throw new EntityNotFoundException("User not found.");
        }

        if (userResult.Data.Wallet == null || userResult.Data.Wallet.AmountOfCoins == null)
        {
            var walletResult = await _unitOfWork.WalletRepository.GetSingleByConditionAsync(w => w.Id == userResult.Data.WalletId);
            if (!walletResult.IsSuccess) throw new EntityNotFoundException("Wallet not found for user.");

            var coinsResult = await _unitOfWork.CoinRepository.GetListByConditionAsync(c => c.WalletId == userResult.Data.WalletId);
            if (!coinsResult.IsSuccess) throw new InvalidOperationException("Failed to load coins for wallet.");

            userResult.Data.Wallet = walletResult.Data;
            userResult.Data.Wallet.AmountOfCoins = coinsResult.Data.ToList();
        }

        return userResult.Data;
    }

    private TransactionHistory CreateHistoryEntry(int userId, TransactionType type, double usdValueChange, NameOfCoin? coinName = null, double coinAmount = 0, double pricePerCoin = 0, string? notes = null)
    {
        return new TransactionHistory
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Type = type,
            CoinName = coinName,
            CoinAmount = coinAmount,
            PricePerCoin = pricePerCoin,
            UsdValueChange = usdValueChange,
            Notes = notes
        };
    }

    #endregion
}
