using CryptexAPI.Context;
using CryptexAPI.Models;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class FuethersDealRepository(AppDbContext context) : BaseRepository<FuethersDeal>(context), IFuethersDealRepository;
