using CryptexAPI.Context;
using CryptexAPI.Exceptions;
using CryptexAPI.Helpers;
using CryptexAPI.Messages;
using CryptexAPI.Models;
using CryptexAPI.Models.Base;
using CryptexAPI.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CryptexAPI.Repos;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    private async Task<Result<TResult>> PerformOperationAsync<TResult>(Func<Task<TResult>> operation, string errorMessage)
    {
        try
        {
            var result = await operation();
            return Result<TResult>.Success(result);
        }
        catch (ArgumentNullException ex)
        {
            return Result<TResult>.Fail(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result<TResult>.Fail(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return Result<TResult>.Fail(ex.Message);
        }
        catch (Exception)
        {
            return Result<TResult>.Fail(errorMessage);
        }
    }

    public async Task<Result<IEnumerable<T>>> GetListByConditionAsync(Expression<Func<T, bool>> condition)
    {
        return await PerformOperationAsync(() =>
        {
            var query = _context.Set<T>().AsQueryable()
                .IncludeAllRecursive(_context)
                .Where(condition);

            return Task.FromResult<IEnumerable<T>>(query);
        }, RepositoryMessages.FailedGetListOfEntityMessage);
    }

    public async Task<Result<T>> GetSingleByConditionAsync(Expression<Func<T, bool>> condition)
    {
        return await PerformOperationAsync(async () =>
        {
            var query = _context.Set<T>().AsQueryable()
                .IncludeAllRecursive(_context);

            var item = await query.FirstOrDefaultAsync(condition);

            if (item == null)
                throw new EntityNotFoundException(RepositoryMessages.FailedGetSingleItemMessage);

            return item;
        }, RepositoryMessages.FailedGetSingleItemMessage);
    }

    public async Task<Result<T>> AddAsync(T item)
    {
        return await PerformOperationAsync(async () =>
        {
            await _context.Set<T>().AddAsync(item);
            return item;
        }, RepositoryMessages.FailedAddItemMessage);
    }

    public async Task<Result<bool>> UpdateAsync(T item, Expression<Func<T, bool>> condition)
    {
        return await PerformOperationAsync(async () =>
        {
            var existingItem = await _context.Set<T>().FirstOrDefaultAsync(condition);

            if (existingItem == null)
            {
                throw new EntityNotFoundException(RepositoryMessages.FailedUpdateItemMessage);
            }

            _context.Entry(existingItem).CurrentValues.SetValues(item)
                ;
            return true;
        }, RepositoryMessages.FailedUpdateItemMessage);
    }

    public async Task<Result<bool>> DeleteAsync(Expression<Func<T, bool>> condition)
    {
        return await PerformOperationAsync(async () =>
        {
            var itemsToRemove = await _context.Set<T>().Where(condition).ToListAsync();

            if (!itemsToRemove.Any())
            {
                throw new EntityNotFoundException(RepositoryMessages.FailedDeleteItemMessage);
            }

            _context.Set<T>().RemoveRange(itemsToRemove);

            return true;
        }, RepositoryMessages.FailedDeleteItemMessage);
    }
}
