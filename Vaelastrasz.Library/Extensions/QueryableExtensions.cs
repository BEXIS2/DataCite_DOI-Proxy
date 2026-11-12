using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Vaelastrasz.Library.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> queryable, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return queryable;

            return queryable;
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            return queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> queryable, string? sortBy) where T : class
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return queryable;

            var allowedProperties = new HashSet<string>(
                typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name),
                StringComparer.OrdinalIgnoreCase
            );

            var sortExpressions = new List<string>();

            foreach (var part in sortBy.Split(',', (char)StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmedPart = part.Trim();
                if (string.IsNullOrEmpty(trimmedPart))
                    continue;

                var tokens = trimmedPart.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0)
                    continue;

                var propertyName = tokens[0];
                if (!allowedProperties.Contains(propertyName))
                    continue;

                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? "descending"
                    : "ascending";

                sortExpressions.Add($"{propertyName} {direction}");
            }

            if (sortExpressions.Count == 0)
                return queryable;

            var sortExpression = string.Join(", ", sortExpressions);
            return queryable.OrderBy(sortExpression);
        }
    }
}