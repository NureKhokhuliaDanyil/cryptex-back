using CryptexAPI.Context;
using CryptexAPI.Models.Persons;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class SupportRepository(AppDbContext context) : BaseRepository<Support>(context), ISupportRepository;