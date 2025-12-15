using CryptexAPI.Models;

namespace CryptexAPI.Services.Interfaces;

public interface IMessageService
{
    Task<Message> CreateMessage(string messageValue, int author, int ticketId);

    Task<List<Message>> GetChatHistoryOfTicket(int ticketId);
}