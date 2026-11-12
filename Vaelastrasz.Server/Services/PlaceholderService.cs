using LiteDB;
using Vaelastrasz.Library.Entities;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Filters;

namespace Vaelastrasz.Server.Services
{
    public class PlaceholderService
    {
        private readonly ConnectionString _connectionString;

        public PlaceholderService(ConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> CreateAsync(string expression, string regularExpression, long userId)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var placeholders = db.GetCollection<Placeholder>("placeholders");
                var users = db.GetCollection<User>("users");

                var user = users.FindById(userId) ?? throw new NotFoundException($"The user (id:{userId}) does not exist.");

                var placeholder = new Placeholder()
                {
                    Expression = expression,
                    RegularExpression = regularExpression,
                    User = user,
                    CreationDate = DateTimeOffset.UtcNow,
                    LastUpdateDate = DateTimeOffset.UtcNow
                };

                return placeholders.Insert(placeholder);
            });
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Placeholder>("placeholders");

                return col.Delete(id);
            });
        }

        public async Task<List<Placeholder>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Placeholder>("placeholders");

                return col.Query().ToList();
            });
        }

        public async Task<List<Placeholder>> QueryAsync(QueryFilter filter)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Placeholder>("placeholders");

                var placeholders = col.Query().ToList().AsQueryable();

                var pageNumber = Math.Max(1, filter.PageNumber);
                var pageSize = Math.Clamp(filter.PageSize, 1, 50);

                placeholders = placeholders.ApplySearch(filter.Search);
                placeholders = placeholders.ApplySort(filter.SortBy);
                placeholders = placeholders.ApplyPagination(pageNumber, pageSize);

                return placeholders.ToList();
            });
        }

        public async Task<Placeholder> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Placeholder>("placeholders");

                var placeholder = col.FindById(id) ?? throw new NotFoundException($"The placeholder (id:{id}) does not exist.");

                return placeholder;
            });
        }

        public async Task<List<Placeholder>> GetByUserIdAsync(long userId)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Placeholder>("placeholders");

                var placeholders = col.Query().Where(p => p.User.Id == userId).ToList();

                return placeholders;
            });
        }

        public async Task<List<Placeholder>> GetByUserIdAsync(long userId, QueryFilter filter)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Placeholder>("placeholders");

                var pageNumber = Math.Max(1, filter.PageNumber);
                var pageSize = Math.Clamp(filter.PageSize, 1, 50);

                var placeholders = col.Query().Where(p => p.User.Id == userId).ToList().AsQueryable();

                placeholders = placeholders.ApplySearch(filter.Search);
                placeholders = placeholders.ApplySort(filter.SortBy);
                placeholders = placeholders.ApplyPagination(pageNumber, pageSize);

                return placeholders.ToList();
            });
        }

        public async Task<bool> UpdateByIdAsync(long id, string expression, string regularExpression, long userId)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var placeholders = db.GetCollection<Placeholder>("placeholders");
                var users = db.GetCollection<User>("users");

                var placeholder = placeholders.FindById(id) ?? throw new NotFoundException($"The placeholder (id:{id}) does not exist.");

                var user = users.FindById(userId) ?? throw new NotFoundException($"The user (id:{userId}) does not exist.");

                placeholder.Expression = expression;
                placeholder.RegularExpression = regularExpression;
                placeholder.User = user;
                placeholder.LastUpdateDate = DateTimeOffset.UtcNow;

                return placeholders.Update(placeholder);
            });
        }
    }
}