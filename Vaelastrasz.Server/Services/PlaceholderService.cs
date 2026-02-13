using LiteDB;
using Vaelastrasz.Library.Entities;
using Vaelastrasz.Library.Exceptions;

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