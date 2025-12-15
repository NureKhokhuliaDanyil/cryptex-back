using CryptexAPI.Enums;
using CryptexAPI.Models;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services;

public class TicketService : ITicketService
{
    private readonly IMessageService _messageService;
    private readonly IUnitOfWork _unitOfWork;
    public TicketService(IMessageService messageService, IUnitOfWork unitOfWork)
    {
        _messageService = messageService;
        _unitOfWork = unitOfWork;
    }
    public async Task<Ticket> CreateTicket(int userId)
    {
        try
        {
            var ticket = new Ticket { ChatHistory = [], Status = Status.Open, UserId = userId };
            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return ticket;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to create ticket. {e.Message}");
        }
    }
    public async Task SendMessageToTicket(int ticketId, int author, string messageValue)
    {
        try
        {
            var result = await _unitOfWork.TicketRepository
                .GetSingleByConditionAsync(e => e.Id == ticketId);
            if (!result.IsSuccess)
            {
                throw new Exception("Failed to get ticket");
            }

            var ticket = result.Data;
            var message = await _messageService.CreateMessage(messageValue, author, ticketId);
            ticket.ChatHistory.Add(message);
            await _unitOfWork.TicketRepository.UpdateAsync(ticket, e => e.Id == ticket.Id);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to send message {e.Message}");
        }
    }
    public async Task<Ticket> GetTicketById(int ticketId)
    {
        try
        {
            var ticket = await _unitOfWork.TicketRepository
                .GetSingleByConditionAsync(e => e.Id == ticketId);
            ticket.Data.ChatHistory = await _messageService.GetChatHistoryOfTicket(ticket.Data.Id);

            return ticket.Data;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to get ticket");
        }
    }
}