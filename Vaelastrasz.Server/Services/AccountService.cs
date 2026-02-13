using LiteDB;
using Vaelastrasz.Library.Entities;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Types;

namespace Vaelastrasz.Server.Services
{
    public class AccountService
    {
        private readonly ConnectionString _connectionString;

        public AccountService(ConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> CreateAsync(string name, string password, string host, string prefix, AccountType accountType)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var accounts = db.GetCollection<Account>("accounts");

                if (accounts.Exists(u => u.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && u.Host.Equals(host, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ConflictException($"The account (name:{name}, host: {host}) already exists.");

                var account = new Account()
                {
                    Name = name,
                    Password = password,
                    Host = host,
                    Prefix = prefix,
                    AccountType = accountType,
                    CreationDate = DateTimeOffset.UtcNow,
                    LastUpdateDate = DateTimeOffset.UtcNow
                };

                return accounts.Insert(account);
            });
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Account>("accounts");

                return col.Delete(id);
            });
        }

        public async Task<List<Account>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Account>("accounts");

                return col.Query().ToList();
            });
        }

        public async Task<Account> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<Account>("accounts");

                var account = col.FindById(id) ?? throw new NotFoundException($"The account (id:{id}) does not exist.");
                return account;
            });
        }

        public async Task<bool> UpdateByIdAsync(long id, string name, string password, string host, string prefix)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var accounts = db.GetCollection<Account>("accounts");

                var account = accounts.FindById(id);

                if (account == null)
                    throw new NotFoundException($"The account (id:{id}) does not exist.");

                account.Prefix = prefix;
                account.Name = name;
                account.Password = password;
                account.Host = host;
                account.LastUpdateDate = DateTimeOffset.UtcNow;

                return accounts.Update(account);
            });
        }
    }
}