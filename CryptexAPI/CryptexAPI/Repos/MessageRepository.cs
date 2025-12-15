using CryptexAPI.Context;
using CryptexAPI.Models;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class MessageRepository(AppDbContext context) : BaseRepository<Message>(context), IMessageRepository;