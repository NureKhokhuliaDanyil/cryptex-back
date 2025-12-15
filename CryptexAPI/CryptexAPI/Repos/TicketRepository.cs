using CryptexAPI.Context;
using CryptexAPI.Models;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class TicketRepository(AppDbContext context) : BaseRepository<Ticket>(context), ITicketRepository;