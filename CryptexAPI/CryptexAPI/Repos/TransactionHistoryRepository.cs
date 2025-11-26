using CryptexAPI.Context;
using CryptexAPI.Models;
using CryptexAPI.Repos.Interfaces;

namespace CryptexAPI.Repos;

public class TransactionHistoryRepository(AppDbContext context) : BaseRepository<TransactionHistory>(context), ITransactionHistoryRepository {}
