using CryptexAPI.Enums;
using CryptexAPI.Exceptions;
using CryptexAPI.Models;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services
{
    public class FuethersDealService : IFuethersDealService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBinanceRequestService _binanceRequestService;

        public FuethersDealService(IUnitOfWork unitOfWork, IBinanceRequestService binanceRequestService)
        {
            _unitOfWork = unitOfWork;
            _binanceRequestService = binanceRequestService;
        }

        public async Task<FuethersDeal> CreateDeal(Coin coin, TypeOfFuetersDeal typeOfFuetersDeal,
            int leverage, int userId, double stopLoss, double takeProfit, double marginValue, double amount)
        {
            try
            {
                var userResult = await _unitOfWork.UserRepository
                    .GetSingleByConditionAsync(e => e.Id == userId);

                if (!userResult.IsSuccess)
                {
                    throw new EntityNotFoundException("User not found.");
                }

                var user = userResult.Data;

                if (user.Balance < marginValue)
                {
                    throw new InvalidOperationException("Insufficient balance to open deal.");
                }

                var fuethersDeal = new FuethersDeal
                {
                    CoinId = coin.Id,
                    EnterPrice = coin.Price,
                    Leverage = leverage,
                    UserId = userId,
                    StopLoss = stopLoss,
                    TakeProfit = takeProfit,
                    TypeOfDeal = typeOfFuetersDeal,
                    TypeOfMargin = TypeOfMargin.Isolate,
                    MarginValue = marginValue,
                    Status = Status.InProcess,
                    Amount = amount
                };

                await _unitOfWork.FuethersDealRepository.AddAsync(fuethersDeal);

                user.Balance -= marginValue;
                await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == user.Id);
                await _unitOfWork.SaveChangesAsync();

                return fuethersDeal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FuethersDeal> CheckFuethersDeal(int dealId)
        {
            try
            {
                var result = await _unitOfWork.FuethersDealRepository
                    .GetSingleByConditionAsync(e => e.Id == dealId);

                if (!result.IsSuccess)
                {
                    throw new EntityNotFoundException("Failed to get deal.");
                }

                var deal = result.Data;

                if (deal.Status == Status.Closed)
                {
                    return deal;
                }

                var coinResult = await _unitOfWork.CoinRepository.GetSingleByConditionAsync(e => e.Id == deal.CoinId);
                var userResult = await _unitOfWork.UserRepository.GetSingleByConditionAsync(e => e.Id == deal.UserId);

                if (!coinResult.IsSuccess || !userResult.IsSuccess)
                {
                    throw new EntityNotFoundException("Failed to get user or coin for the deal.");
                }

                var coin = coinResult.Data;
                var user = userResult.Data;

                var currentPrice = await _binanceRequestService.GetCoinPriceFromBinance(coin.Name);

                var pnl = CalculatePnl(deal, currentPrice);
                bool dealClosed = false;

                if (pnl < 0 && Math.Abs(pnl) >= deal.MarginValue)
                {
                    deal.Status = Status.Closed;
                    deal.MarginValue = 0;
                    dealClosed = true;
                }

                else if ((deal.TypeOfDeal == TypeOfFuetersDeal.Long && currentPrice <= deal.StopLoss) ||
                         (deal.TypeOfDeal == TypeOfFuetersDeal.Short && currentPrice >= deal.StopLoss))
                {
                    deal.Status = Status.Closed;
                    user.Balance += deal.MarginValue + pnl; 
                    dealClosed = true;
                }

                else if ((deal.TypeOfDeal == TypeOfFuetersDeal.Long && currentPrice >= deal.TakeProfit) ||
                         (deal.TypeOfDeal == TypeOfFuetersDeal.Short && currentPrice <= deal.TakeProfit))
                {
                    deal.Status = Status.Closed;
                    user.Balance += deal.MarginValue + pnl;
                    dealClosed = true;
                }

                if (dealClosed)
                {
                    await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == user.Id);
                    await _unitOfWork.FuethersDealRepository.UpdateAsync(deal, e => e.Id == deal.Id);
                    await _unitOfWork.SaveChangesAsync();
                }

                return deal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<FuethersDeal> CloseDeal(int dealId)
        {
            try
            {
                var deal = await CheckFuethersDeal(dealId);

                if (deal.Status == Status.Closed)
                {
                    return deal;
                }

                var coinResult = await _unitOfWork.CoinRepository.GetSingleByConditionAsync(e => e.Id == deal.CoinId);
                var userResult = await _unitOfWork.UserRepository.GetSingleByConditionAsync(e => e.Id == deal.UserId);

                if (!coinResult.IsSuccess || !userResult.IsSuccess)
                {
                    throw new EntityNotFoundException("Failed to get user or coin for the deal.");
                }

                var coin = coinResult.Data;
                var user = userResult.Data;

                var currentPrice = await _binanceRequestService.GetCoinPriceFromBinance(coin.Name);
                var pnl = CalculatePnl(deal, currentPrice);

                user.Balance += deal.MarginValue + pnl;
                deal.Status = Status.Closed;

                await _unitOfWork.UserRepository.UpdateAsync(user, e => e.Id == user.Id);
                await _unitOfWork.FuethersDealRepository.UpdateAsync(deal, e => e.Id == deal.Id);
                await _unitOfWork.SaveChangesAsync();

                return deal;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to close the Deal {e.Message}");
            }
        }

        public async Task<List<FuethersDeal>> GetAllFuethersDealsForUser(int userId)
        {
            try
            {
                var deals = await _unitOfWork.FuethersDealRepository
                    .GetListByConditionAsync(e => e.UserId == userId);

                if (!deals.IsSuccess)
                {
                    return new List<FuethersDeal>(); 
                }

                return deals.Data.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get deals for this user: {e.Message}");
            }
        }

        private double CalculatePnl(FuethersDeal deal, double currentPrice)
        {
            if (deal.TypeOfDeal == TypeOfFuetersDeal.Long)
            {
                return (currentPrice - deal.EnterPrice) * deal.Amount * deal.Leverage;
            }
            else
            {
                return (deal.EnterPrice - currentPrice) * deal.Amount * deal.Leverage;
            }
        }
    }
}
