using LiteDB;
using Vaelastrasz.Library.Entities;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Server.Utilities;

namespace Vaelastrasz.Server.Services
{
    public class UserService
    {
        private readonly ConnectionString _connectionString;

        public UserService(ConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> CreateAsync(string name, string password, string project, string pattern, long accountId, bool isActive)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var users = db.GetCollection<User>("users");
                var accounts = db.GetCollection<Account>("accounts");

                if (users.Exists(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    throw new ConflictException($"The user (name:{name}) already exists.");

                var account = accounts.FindById(accountId) ?? throw new NotFoundException($"The account (id:{accountId}) does not exist.");

                //if(!pattern.IsValidRegex())
                //    throw new BadRequestException($"The pattern ({pattern}) is not a valid regex.");

                // salt
                var salt = CryptographyUtils.GetRandomBase64String(16);

                var user = new User
                {
                    Name = name,
                    Salt = salt,
                    Password = CryptographyUtils.GetSHA512HashAsBase64(salt, password),
                    Pattern = pattern,
                    Project = project,
                    IsActive = isActive,
                    CreationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    Account = account
                };

                return users.Insert(user);
            });
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<User>("users");

                return col.Delete(id);
            });
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            using var db = new LiteDatabase(_connectionString);
            var col = db.GetCollection<User>("users");

            var users = col.Find(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (users.Count() != 1)
                return await Task.FromResult(false);

            return await Task.FromResult(true);
        }

        public async Task<List<User>> GetAsync()
        {
            List<User> users = new List<User>();

            using var db = new LiteDatabase(_connectionString);
            var col = db.GetCollection<User>("users");
            users = col.Include(u => u.Account).Query().ToList();

            return await Task.FromResult(users.ToList());
        }

        public async Task<User> GetByIdAsync(long id)
        {
            using var db = new LiteDatabase(_connectionString);
            var col = db.GetCollection<User>("users");

            var user = col.Include(u => u.Account).FindById(id);

            return await Task.FromResult(user) ?? throw new NotFoundException($"The user (id:{id}) does not exist.");
        }

        public async Task<User> GetByNameAsync(string name)
        {
            using var db = new LiteDatabase(_connectionString);
            var col = db.GetCollection<User>("users");

            var users = col.Include(u => u.Account).Find(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (users.Count() == 0)
                throw new NotFoundException($"The user (name:{name}) does not exist.");

            if (users.Count() > 1)
                throw new ConflictException($"The user (name:{name}) exists more than once.");

            return await Task.FromResult(users.Single());
        }

        public async Task<bool> UpdateByIdAsync(long id, string name, string password, string project, string pattern, long accountId, bool isActive)
        {
            using var db = new LiteDatabase(_connectionString);
            var users = db.GetCollection<User>("users");
            var accounts = db.GetCollection<Account>("accounts");

            var user = users.FindById(id) ?? throw new NotFoundException($"The user (id:{id}) does not exist.");
            var account = accounts.FindById(accountId) ?? throw new NotFoundException($"The account (id:{accountId}) does not exist.");
            var salt = CryptographyUtils.GetRandomBase64String(16);

            user.Name = name;
            user.Salt = salt;
            user.Password = CryptographyUtils.GetSHA512HashAsBase64(salt, password);
            user.Project = project;
            user.Pattern = pattern;
            user.Account = account;
            user.IsActive = isActive;
            user.LastUpdateDate = DateTimeOffset.UtcNow;

            return await Task.FromResult(users.Update(user));
        }

        public async Task<bool> VerifyAsync(string name, string password)
        {
            using var db = new LiteDatabase(_connectionString);
            var users = db.GetCollection<User>("users");

            var user = users.FindOne(u => u.Name == name);

            return user == null
                ? throw new NotFoundException($"The user (name:{name}) does not exist.")
                : await Task.FromResult(user.Password == CryptographyUtils.GetSHA512HashAsBase64(user.Salt, password));
        }
    }
}