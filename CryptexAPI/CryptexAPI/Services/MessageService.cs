using CryptexAPI.Models;
using CryptexAPI.Services.Interfaces;
using CryptexAPI.UnitOfWork;

namespace CryptexAPI.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Message> CreateMessage(string valueOfMessage, int authorId, int idOfTicket)
    {
        try
        {
            var message = new Message() { AuthorId = authorId, Value = valueOfMessage, TicketId = idOfTicket };
            await _unitOfWork.MessageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return message;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to create message. {e.Message}");
        }
    }

    public async Task<List<Message>> GetChatHistoryOfTicket(int ticketId)
    {
        try
        {
            var chatHistory = await _unitOfWork.MessageRepository
                .GetListByConditionAsync(e => e.TicketId == ticketId);

            return (List<Message>)chatHistory.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}