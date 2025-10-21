using CryptexAPI.Context;
using CryptexAPI.Models.Persons;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class UserRepository(AppDbContext context) 
    : BaseRepository<User>(context), IUserRepository;
