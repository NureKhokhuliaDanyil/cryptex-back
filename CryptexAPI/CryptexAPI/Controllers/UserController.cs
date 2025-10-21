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
}