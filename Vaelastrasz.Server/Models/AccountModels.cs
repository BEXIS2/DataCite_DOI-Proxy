using System.ComponentModel.DataAnnotations;
using Vaelastrasz.Library.Entities;
using Vaelastrasz.Library.Types;

namespace Vaelastrasz.Server.Models
{
    public class CreateAccountModel
    {
        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        public string Host { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Prefix { get; set; }
    }

    public class ReadAccountModel
    {
        public AccountType AccountType { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public string Host { get; set; }
        public long Id { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Prefix { get; set; }

        public static ReadAccountModel Convert(Account account)
        {
            return new ReadAccountModel()
            {
                Id = account.Id,
                Name = account.Name,
                Password = account.Password,
                Host = account.Host,
                Prefix = account.Prefix,
                CreationDate = account.CreationDate,
                LastUpdateDate = account.LastUpdateDate,
                AccountType = account.AccountType
            };
        }
    }

    public class UpdateAccountModel
    {
        public AccountType AccountType { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Prefix { get; set; }
    }
}