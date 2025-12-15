using CryptexAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptexAPI.Controllers;

[Authorize(Roles = "Support, Admin")]
[Route("api/support")]
[ApiController]
public class SupportController : ControllerBase
{
    private readonly ISupportService _supportService;

    public SupportController(ISupportService supportService)
    {
        _supportService = supportService;
    }

    [HttpPost("take-ticket")]
    public async Task<IActionResult> TakeTicket([FromQuery] int supportId, [FromQuery] int ticketId)
    {
        try
        {
            await _supportService.TakeTicket(supportId, ticketId);
            return Ok("Ticket assigned successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to assign ticket: {ex.Message}");
        }
    }

    [HttpPost("resolve-ticket")]
    public async Task<IActionResult> ResolveTicket([FromQuery] int supportId, [FromQuery] int ticketId)
    {
        try
        {
            await _supportService.CloseTicket(ticketId, supportId);
            return Ok("Ticket resolved successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to resolve ticket: {ex.Message}");
        }
    }

    [HttpGet("current-ticket")]
    public async Task<IActionResult> GetCurrentTicket([FromQuery] int supportId)
    {
        try
        {
            var ticket = await _supportService.GetCurrentTicket(supportId);
            return Ok(ticket);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to get current ticket: {ex.Message}");
        }
    }

    [HttpGet("get-open-tickets")]
    public async Task<IActionResult> GetOpenTickets()
    {
        try
        {
            var tickets = await _supportService.GetOpenTickets();
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to get open tickets: {ex.Message}");
        }
    }
}