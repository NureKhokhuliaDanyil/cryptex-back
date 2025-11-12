using CryptexAPI.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace CryptexAPI.Controllers;

[Route("api/jobs")]
[ApiController]
public class JobController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public JobController(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    [HttpPost("update-prices")]
    public async Task<IActionResult> UpdateCoinPrices()
    {
        var expectedSecret = _configuration["CronJobSecret"];
        if (string.IsNullOrEmpty(expectedSecret) ||
            !Request.Headers.TryGetValue("x-api-key", out var receivedSecret) ||
            receivedSecret != expectedSecret)
        {
            return Unauthorized("Invalid or missing API key.");
        }

        try
        {
            await _unitOfWork.CoinRepository.UpdatePricesFromBinance();
            return Ok("Coin prices updated successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating prices: {ex.Message}");
        }
    }
}
