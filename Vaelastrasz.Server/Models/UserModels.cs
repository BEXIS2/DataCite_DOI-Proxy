using System.ComponentModel.DataAnnotations;
using Vaelastrasz.Library.Entities;

namespace Vaelastrasz.Server.Models
{
    public class CreateUserModel
    {
        [Required]
        public long AccountId { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Password { get; set; }

        public required string Pattern { get; set; }
        public required string Project { get; set; }
    }

    public class LoginUserModel
    {
        public required string Password { get; set; }
        public required string Username { get; set; }
    }

    public class ReadUserModel
    {
        public long? AccountId { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public required string Name { get; set; }
        public required string Pattern { get; set; }
        public required string Project { get; set; }

        public static ReadUserModel Convert(User user)
        {
            return new ReadUserModel()
            {
                Id = user.Id,
                Name = user.Name,
                Pattern = user.Pattern,
                CreationDate = user.CreationDate,
                LastUpdateDate = user.LastUpdateDate,
                AccountId = user.Account?.Id,
                IsActive = user.IsActive,
                Project = user.Project
            };
        }
    }

    public class UpdateUserModel
    {
        public long AccountId { get; set; }
        public bool IsActive { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Pattern { get; set; }
        public required string Project { get; set; }
    }
}