using CryptexAPI.Enums;
using CryptexAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptexAPI.Controllers;

[Authorize(Roles = "User, Support, Admin")]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/wallet/balance")]
    public async Task<IActionResult> GetTotalWalletBalance(int id)
    {
        try
        {
            var balance = await _userService.GetTotalWalletBalance(id);
            return Ok(balance);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/wallet")]
    public async Task<IActionResult> GetMyWallet(int id)
    {
        try
        {
            var wallet = await _userService.GetMyWallet(id);
            return Ok(wallet);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/wallet/buy")]
    public async Task<IActionResult> BuyCoin(int id, [FromQuery] NameOfCoin coin, [FromQuery] double amount)
    {
        try
        {
            await _userService.BuyCoin(id, coin, amount);
            var user = await _userService.GetById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/wallet/sell")]
    public async Task<IActionResult> SellCoin(int id, [FromQuery] NameOfCoin coin, [FromQuery] double amount)
    {
        try
        {
            await _userService.SellCoin(id, coin, amount);
            var user = await _userService.GetById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/wallet/convert")]
    public async Task<IActionResult> ConvertCurrency(
        int id,
        [FromQuery] NameOfCoin coinForConvert,
        [FromQuery] NameOfCoin convertToCoin,
        [FromQuery] double amount)
    {
        try
        {
            await _userService.ConvertCurrency(id, coinForConvert, convertToCoin, amount);
            var user = await _userService.GetById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{userId}/change-balance")]
    public async Task<IActionResult> ChangeUserBalance(int userId, [FromQuery] double amount)
    {
        try
        {
            var user = await _userService.ChangeBalance(userId, amount);
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Error: {e.Message}" });
        }
    }

    [HttpPatch("{userId}/deposit")]
    public async Task<IActionResult> DepositFunds(int userId, [FromQuery] double amount)
    {
        try
        {
            var user = await _userService.DepositFundsAsync(userId, amount);
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Error: {e.Message}" });
        }
    }

    [HttpPatch("{userId}/deposit-crypto")]
    public async Task<IActionResult> DepositCrypto(
        int userId,
        [FromQuery] string depositAddress,
        [FromQuery] double amount)
    {
        try
        {
            var user = await _userService.DepositCryptoAsync(userId, depositAddress, amount);
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Error depositing crypto: {e.Message}" });
        }
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> WithdrawFunds(int id, [FromQuery] double amount)
    {
        try
        {
            await _userService.WithdrawFundsAsync(id, amount);
            return Ok("Funds were withdrawn successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/withdraw-crypto")]
    public async Task<IActionResult> WithdrawCrypto(
        int id,
        [FromQuery] NameOfCoin coinName,
        [FromQuery] double amount,
        [FromQuery] string externalAddress)
    {
        try
        {
            await _userService.WithdrawCryptoAsync(id, coinName, amount, externalAddress);
            return Ok($"Withdrawal of {amount} {coinName} to {externalAddress} initiated successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error withdrawing crypto: {ex.Message}" });
        }
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetTransactionHistory(int id)
    {
        try
        {
            var history = await _userService.GetTransactionHistoryAsync(id);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}