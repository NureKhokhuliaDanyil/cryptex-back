using CryptexAPI.Enums;
using CryptexAPI.Exceptions;
using CryptexAPI.Helpers;
using CryptexAPI.Helpers.Constants;
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

    private readonly ITicketService _ticketService;

    private readonly IEmailService _emailService;

    public UserService(IWalletService walletService, IUnitOfWork unitOfWork, ITicketService ticketService, IEmailService emailService)
    {
        _walletService = walletService;
        _unitOfWork = unitOfWork;
        _ticketService = ticketService;
        _emailService = emailService;
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

            User userEntity;

            if (parsedRole == Role.Support)
            {
                userEntity = new Support
                {
                    Experience = 0,
                    Salary = 200
                };
            }
            else
            {
                userEntity = new User();
            }

            userEntity.GoogleID = registrationModel.GoogleID;
            userEntity.Email = registrationModel.Email;
            userEntity.Name = registrationModel.Name;
            userEntity.Surname = registrationModel.Surname;
            userEntity.PhoneNumber = registrationModel.PhoneNumber;
            userEntity.Wallet = wallet;
            userEntity.WalletId = wallet.Id;
            userEntity.Age = registrationModel.Age;
            userEntity.Country = registrationModel.Country;
            userEntity.Adress = registrationModel.Adress;
            userEntity.Role = parsedRole;

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
            var user = await GetUserByIdAsync(id);
            var coinInWallet = user.Wallet.AmountOfCoins.FirstOrDefault(c => c.Name == coin);

            if (coinInWallet == null)
            {
                throw new EntityNotFoundException($"Coin {coin} not found in user's wallet");
            }

            var currentPrice = coinInWallet.Price;
            if (currentPrice <= 0)
            {
                throw new InvalidOperationException($"Cannot buy {coin}, price is zero. Please update prices.");
            }

            var cost = currentPrice * amount;

            if (user.Balance == null || cost > user.Balance)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            user.Balance -= cost;
            coinInWallet.Amount += amount;

            var history = CreateHistoryEntry(id, TransactionType.Buy, -cost, coin, amount, currentPrice);
            await _unitOfWork.TransactionHistoryRepository.AddAsync(history);

            await _unitOfWork.CoinRepository.UpdateAsync(coinInWallet, c => c.Id == coinInWallet.Id);
            await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == id);
            await _unitOfWork.SaveChangesAsync();


            //await _emailService.SendEmail(user.Email, EmailStrings.BuyCoinSubject,
            //    EmailStrings.GetBuyCoinBody(
            //        user.Name,
            //        user.Surname,
            //        coin, amount,
            //        user.Balance
            //    )
            //);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SellCoin(int id, NameOfCoin coin, double amount)
    {
        try
        {
            var user = await GetUserByIdAsync(id);
            var coinInWallet = user.Wallet.AmountOfCoins.FirstOrDefault(c => c.Name == coin);

            if (coinInWallet == null)
            {
                throw new EntityNotFoundException($"Coin {coin} not found in user's wallet");
            }

            if (coinInWallet.Amount < amount)
            {
                throw new InvalidOperationException("Insufficient coin amount to sell.");
            }

            var currentPrice = coinInWallet.Price;
            var income = currentPrice * amount;

            user.Balance = (user.Balance ?? 0) + income;
            coinInWallet.Amount -= amount;

            var history = CreateHistoryEntry(id, TransactionType.Sell, income, coin, -amount, currentPrice);
            await _unitOfWork.TransactionHistoryRepository.AddAsync(history);

            await _unitOfWork.CoinRepository.UpdateAsync(coinInWallet, c => c.Id == coinInWallet.Id);
            await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == id);
            await _unitOfWork.SaveChangesAsync();

            //await _emailService.SendEmail(user.Email, EmailStrings.SellCoinSubject,
            //    EmailStrings.GetSellCoinBody(
            //        user.Name,
            //        user.Surname,
            //        coin, amount,
            //        user.Balance
            //    )
            //);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task ConvertCurrency(int idOfUser, NameOfCoin coinForConvert, NameOfCoin imWhichCoinConvert, double amountOfCoinForConvert)
    {
        try
        {
            var user = await GetUserByIdAsync(idOfUser);

            var coinFrom = user.Wallet.AmountOfCoins.FirstOrDefault(e => e.Name == coinForConvert);
            var coinTo = user.Wallet.AmountOfCoins.FirstOrDefault(e => e.Name == imWhichCoinConvert);

            if (coinFrom == null || coinTo == null)
            {
                throw new EntityNotFoundException("One or both coins were not found in the wallet.");
            }

            if (coinFrom.Amount < amountOfCoinForConvert)
            {
                throw new InvalidOperationException("Not enough coins for conversion.");
            }

            if (coinTo.Price <= 0)
            {
                throw new InvalidOperationException($"Cannot convert to {coinTo.Name}, its price is zero.");
            }

            var amountAfterConversion = (coinFrom.Price / coinTo.Price) * amountOfCoinForConvert;
            var usdValue = coinFrom.Price * amountOfCoinForConvert; 

            coinFrom.Amount -= amountOfCoinForConvert;
            coinTo.Amount += amountAfterConversion;

            var historyFrom = CreateHistoryEntry(idOfUser, TransactionType.Convert, 0, coinFrom.Name, -amountOfCoinForConvert, coinFrom.Price, $"To {coinTo.Name}");
            var historyTo = CreateHistoryEntry(idOfUser, TransactionType.Convert, 0, coinTo.Name, amountAfterConversion, coinTo.Price, $"From {coinFrom.Name}");

            await _unitOfWork.TransactionHistoryRepository.AddAsync(historyFrom);
            await _unitOfWork.TransactionHistoryRepository.AddAsync(historyTo);

            await _unitOfWork.CoinRepository.UpdateAsync(coinFrom, c => c.Id == coinFrom.Id);
            await _unitOfWork.CoinRepository.UpdateAsync(coinTo, c => c.Id == coinTo.Id);
            await _unitOfWork.SaveChangesAsync();

            //await _emailService.SendEmail(user.Email, EmailStrings.ConvertCurrencySubject,
            //    EmailStrings.GetConvertCurrencyBody(user.Name, user.Surname, coinForConvert,
            //        coinFrom.Amount, imWhichCoinConvert, coinTo.Amount));
        }
        catch (Exception)
        {
            throw;
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

    public async Task<User> DepositCryptoAsync(int userId, string depositAddress, double amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException($"Deposit amount must be positive.");
        }

        var user = await GetUserByIdAsync(userId);
        var coinInWallet = user.Wallet.AmountOfCoins.FirstOrDefault(c => c.DepositAddress == depositAddress);

        if (coinInWallet == null)
        {
            throw new EntityNotFoundException($"Deposit address {depositAddress} not found in user's wallet.");
        }

        coinInWallet.Amount += amount;
        var usdValueChange = coinInWallet.Price * amount;

        var history = CreateHistoryEntry(
            userId,
            TransactionType.Deposit,
            usdValueChange,
            coinInWallet.Name,
            amount,
            coinInWallet.Price,
            $"Crypto deposit of {amount} {coinInWallet.Name}");

        await _unitOfWork.TransactionHistoryRepository.AddAsync(history);

        await _unitOfWork.CoinRepository.UpdateAsync(coinInWallet, c => c.Id == coinInWallet.Id);
        await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == userId);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task WithdrawCryptoAsync(int userId, NameOfCoin coinName, double amount, string externalAddress)
    {
        if (amount <= 0)
        {
            throw new ArgumentException($"Withdrawal amount for {coinName} must be positive.");
        }

        var user = await GetUserByIdAsync(userId);
        var coinInWallet = user.Wallet.AmountOfCoins.FirstOrDefault(c => c.Name == coinName);

        if (coinInWallet == null)
        {
            throw new EntityNotFoundException($"Coin {coinName} not found in user's wallet.");
        }

        if (coinInWallet.Amount < amount)
        {
            throw new InvalidOperationException($"Insufficient {coinName} amount to withdraw.");
        }

        coinInWallet.Amount -= amount;

        var usdValueChange = coinInWallet.Price * amount;
        var notes = $"To external address: {externalAddress}";

        var history = CreateHistoryEntry(
            userId,
            TransactionType.Withdraw,
            -usdValueChange,
            coinName,
            -amount,
            coinInWallet.Price,
            notes);

        await _unitOfWork.TransactionHistoryRepository.AddAsync(history);

        await _unitOfWork.CoinRepository.UpdateAsync(coinInWallet, c => c.Id == coinInWallet.Id);
        await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == userId);
        await _unitOfWork.SaveChangesAsync();
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

    #region Ticket Features

    public async Task<Ticket> CreateTicket(int id)
    {
        try
        {
            var ticket = await _ticketService.CreateTicket(id);
            var user = await _unitOfWork.UserRepository
                .GetSingleByConditionAsync(e => e.Id == id);

            //await _emailService.SendEmail(user.Data.Email, EmailStrings.WelcomeSubject,
            //    EmailStrings.GetTicketCreatedBody(user.Data.Name, user.Data.Surname, ticket.Id, ticket.Status));

            return ticket;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task SendMessageToTicketChat(int idOfTicket, int idOfAuthorOfMessage, string valueOfMessage)
    {
        try
        {
            if (idOfAuthorOfMessage == null)
            {
                throw new Exception("Empty author of ticket");
            }
            await _ticketService.SendMessageToTicket(idOfTicket, idOfAuthorOfMessage, valueOfMessage);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get user {e.Message}");
        }
    }

    public async Task<List<Ticket>> GetAllMyTickets(int userId)
    {
        try
        {
            var tickets = await _unitOfWork.TicketRepository.GetListByConditionAsync(e => e.UserId == userId);
            var updatedTicketsWithChatHistory = tickets.Data.ToList();
            if (tickets == null)
            {
                throw new Exception("Failed to get tickets");
            }

            return updatedTicketsWithChatHistory;
        }
        catch (Exception e)
        {
            throw new Exception($"Error {e.Message}");
        }
    }

    public async Task<Ticket> GetTicketById(int idOfTicket)
    {
        try
        {
            var ticket = await _ticketService.GetTicketById(idOfTicket);
            if (ticket == null)
            {
                throw new Exception("Failed to get ticket");
            }

            return ticket;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get ticket");
        }
    }

    #endregion
}
