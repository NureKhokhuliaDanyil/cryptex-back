using CryptexAPI.Enums;
using CryptexAPI.Models.Base;

namespace CryptexAPI.Models;

public class Ticket : BaseEntity
{
    public Status Status { get; set; }
    public int UserId { get; set; }
    public List<Message> ChatHistory { get; set; }
}