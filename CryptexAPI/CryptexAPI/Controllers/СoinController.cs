using CryptexAPI.Enums;
using CryptexAPI.Models.Wallets;
using CryptexAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptexAPI.Controllers;

[Authorize(Roles = "User, Support, Admin")]
[Route("api/coin")]
[ApiController]
public class СoinController : ControllerBase
{
    private readonly ICoinService _coinService;

    public СoinController(ICoinService coinService)
    {
        _coinService = coinService;
    }

    [HttpGet("price-history")]
    public async Task<ActionResult<List<PriceHistoryItem>>> GetPriceHistory(
        [FromQuery] NameOfCoin coin,
        [FromQuery] BinanceInterval periodOfTime)
    {
        try
        {
            var prices = await _coinService.GetPriceHistory(coin, periodOfTime);
            return Ok(prices);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Error fetching price history: {e.Message}");
        }
    }
}
