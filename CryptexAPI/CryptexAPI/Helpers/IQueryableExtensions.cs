using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CryptexAPI.Helpers;

public static class IQueryableExtensions
{
    public static IQueryable<T> IncludeAllRecursive<T>(this IQueryable<T> query, DbContext context) where T : class
    {
        var entityType = context.Model.FindEntityType(typeof(T));
        var includedPaths = new HashSet<string>();

        void AddIncludes(IEntityType entity, string prefix)
        {
            foreach (var navigation in entity.GetNavigations())
            {
                var path = string.IsNullOrEmpty(prefix) ? navigation.Name : $"{prefix}.{navigation.Name}";

                if (!includedPaths.Contains(path))
                {
                    includedPaths.Add(path);
                }

                var targetEntityType = navigation.TargetEntityType;
                AddIncludes(targetEntityType, path);
            }
        }

        AddIncludes(entityType, "");

        foreach (var includePath in includedPaths)
        {
            query = query.Include(includePath);
        }

        return query;
    }
}
