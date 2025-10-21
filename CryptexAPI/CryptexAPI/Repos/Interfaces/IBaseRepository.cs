using CryptexAPI.Models;
using CryptexAPI.Models.Base;
using System.Linq.Expressions;

namespace CryptexAPI.Repos.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<Result<IEnumerable<T>>> GetListByConditionAsync(Expression<Func<T, bool>> condition);
    Task<Result<T>> GetSingleByConditionAsync(Expression<Func<T, bool>> condition);
    Task<Result<T>> AddAsync(T item);
    Task<Result<bool>> UpdateAsync(T item, Expression<Func<T, bool>> condition);
    Task<Result<bool>> DeleteAsync(Expression<Func<T, bool>> condition);
}
