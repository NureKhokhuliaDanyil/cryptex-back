using CryptexAPI.Models;
using CryptexAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptexAPI.Controllers;

[Authorize(Roles = "User, Support, Admin")]
[Route("api/fuether-deal")]
[ApiController]
public class FuethersDealsController : ControllerBase
{
    private readonly IFuethersDealService _fuethersDealService;

    public FuethersDealsController(IFuethersDealService fuethersDealService)
    {
        _fuethersDealService = fuethersDealService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDeal([FromBody] FuethersDealRequest request)
    {
        var deal = await _fuethersDealService.CreateDeal(
            request.Coin,
            request.TypeOfDeal,
            request.Leverage,
            request.UserId,
            request.StopLoss,
            request.TakeProfit,
            request.MarginValue,
            request.Amount);

        return Ok(deal);
    }

    [HttpGet("check/{dealId}")]
    public async Task<IActionResult> CheckDeal(int dealId)
    {
        var deal = await _fuethersDealService.CheckFuethersDeal(dealId);
        return Ok(deal);
    }

    [HttpPost("close/{dealId}")]
    public async Task<IActionResult> CloseDeal(int dealId)
    {
        var deal = await _fuethersDealService.CloseDeal(dealId);
        return Ok(deal);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserDeals(int userId)
    {
        var deals = await _fuethersDealService.GetAllFuethersDealsForUser(userId);
        return Ok(deals);
    }

}