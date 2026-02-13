using LiteDB;
using Vaelastrasz.Library.Entities;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Models;

namespace Vaelastrasz.Server.Services
{
    public class DOIService
    {
        private readonly ConnectionString _connectionString;

        public DOIService(ConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> CreateAsync(string prefix, string suffix, DOIStateType state, long userId, string value)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var dois = db.GetCollection<DOI>("dois");
                var users = db.GetCollection<User>("users");

                var user = users.Include(u => u.Account).FindById(userId) ?? throw new NotFoundException($"The user (id:{userId}) does not exist.");

                if (!user.Account.Prefix.Equals(prefix, StringComparison.InvariantCultureIgnoreCase))
                    throw new ForbiddenException($"The doi (prefix:{prefix}) is invalid.");

                if (dois.Find(d => d.Prefix.Equals(prefix, StringComparison.InvariantCultureIgnoreCase) && d.Suffix.Equals(suffix, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                    throw new ConflictException($"The doi (prefix:{prefix}, suffix: {suffix}) already exists.");

                var doi = new DOI()
                {
                    Prefix = prefix,
                    Suffix = suffix,
                    State = state,
                    User = user,
                    Value = value,
                    CreationDate = DateTimeOffset.UtcNow,
                    LastUpdateDate = DateTimeOffset.UtcNow
                };

                return dois.Insert(doi);
            });
        }

        public async Task<bool> DeleteByDOIAsync(string doi)
        {
            return await Task.Run(() =>
            {
                if (!doi.Contains('/'))
                    throw new BadRequestException($"The value of doi ({doi}) is invalid.");

                string prefix = doi.Split('/')[0];
                string suffix = doi.Split('/')[1];

                return DeleteByPrefixAndSuffixAsync(prefix, suffix);
            });
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                return col.Delete(id);
            });
        }

        public async Task<bool> DeleteByPrefixAndSuffixAsync(string prefix, string suffix)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var dois = db.GetCollection<DOI>("dois");
                var users = db.GetCollection<User>("users");

                var doi = dois.Find(d => d.Prefix.Equals(prefix, StringComparison.OrdinalIgnoreCase) && d.Suffix.Equals(suffix, StringComparison.OrdinalIgnoreCase));

                if (doi == null || !doi.Any())
                    throw new NotFoundException($"The doi (prefix:{prefix}, suffix: {suffix}) does not exist.");

                if (doi.Count() > 1)
                    throw new ConflictException($"The doi (prefix:{prefix}, suffix: {suffix}) exists more than once.");

                var user = users.FindById(doi.Single().User.Id) ?? throw new NotFoundException($"The user of doi (prefix:{prefix}, suffix: {suffix}) does not exist.");

                if (!user.Account.Prefix.Equals(prefix, StringComparison.OrdinalIgnoreCase))
                    throw new ForbiddenException($"The doi (prefix:{prefix}) is invalid.");

                return dois.Delete(doi.Single().Id);
            });
        }

        public async Task<List<DOI>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                return col.FindAll().ToList();
            });
        }

        public async Task<DOI> GetByDOIAsync(string doi)
        {
            return await Task.Run(() =>
            {
                if (!doi.Contains('/'))
                    throw new BadRequestException($"The value of doi ({doi}) is invalid.");

                string prefix = doi.Split('/')[0];
                string suffix = doi.Split('/')[1];

                return GetByPrefixAndSuffixAsync(prefix, suffix);
            });
        }

        public async Task<DOI> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                var doi = col.FindById(id);

                return doi != null ? doi : throw new NotFoundException($"The doi (id:{id}) does not exist.");
            });
        }

        public async Task<DOI> GetByPrefixAndSuffixAsync(string prefix, string suffix)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                var dois = col.Find(d => d.Prefix.Equals(prefix, StringComparison.OrdinalIgnoreCase) && d.Suffix.Equals(suffix, StringComparison.OrdinalIgnoreCase));

                if (dois == null || !dois.Any())
                    throw new NotFoundException($"The doi (prefix:{prefix}, suffix: {suffix}) does not exist.");

                if (dois.Count() > 1)
                    throw new ConflictException($"The doi (prefix:{prefix}, suffix: {suffix}) exists more than once.");

                return dois.Single();
            });
        }

        public async Task<List<DOI>> GetByPrefixAsync(string prefix)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                return col.Find(d => d.Prefix.Equals(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            });
        }

        public async Task<List<DOI>> GetBySuffixAsync(string suffix)
        {
            return await Task.Run(() =>
            {
                if (suffix == null)
                    throw new ArgumentNullException(nameof(suffix));

                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                return col.Find(d => d.Suffix.Equals(suffix, StringComparison.OrdinalIgnoreCase)).ToList();
            });
        }

        public async Task<List<DOI>> GetByUserIdAsync(long userId)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var col = db.GetCollection<DOI>("dois");

                return col.Find(d => d.User.Id == userId).ToList();
            });
        }

        public async Task<bool> UpdateByDOIAsync(string doi, DOIStateType state, string value)
        {
            return await Task.Run(() =>
            {
                if (!doi.Contains('/'))
                    throw new BadRequestException($"The value of doi ({doi}) is invalid.");

                string prefix = doi.Split('/')[0];
                string suffix = doi.Split('/')[1];

                return UpdateByPrefixAndSuffixAsync(prefix, suffix, state, value);
            });
        }

        public async Task<bool> UpdateByIdAsync(long id, DOIStateType state, string value)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var dois = db.GetCollection<DOI>("dois");

                var doi = dois.FindById(id) ?? throw new NotFoundException($"The doi (id:{id}) does not exist.");

                doi.State = state;
                doi.Value = value;
                doi.LastUpdateDate = DateTimeOffset.UtcNow;

                return dois.Update(doi);
            });
        }

        public async Task<bool> UpdateByPrefixAndSuffixAsync(string prefix, string suffix, DOIStateType state, string value)
        {
            return await Task.Run(() =>
            {
                using var db = new LiteDatabase(_connectionString);
                var dois = db.GetCollection<DOI>("dois");
                var users = db.GetCollection<User>("users");

                var doi = GetByPrefixAndSuffixAsync(prefix, suffix).Result ?? throw new NotFoundException($"The doi (doi:{prefix}/{suffix}) does not exist.");

                doi.State = state;
                doi.Value = value;
                doi.LastUpdateDate = DateTimeOffset.UtcNow;

                return dois.Update(doi);
            });
        }
    }
}