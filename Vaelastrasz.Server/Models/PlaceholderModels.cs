using Vaelastrasz.Library.Entities;

namespace Vaelastrasz.Server.Models
{
    public class CreatePlaceholderModel
    {
        public string Expression { get; set; }
        public string RegularExpression { get; set; }
        public long UserId { get; set; }
    }

    public class ReadPlaceholderModel
    {
        public DateTimeOffset CreationDate { get; set; }
        public string Expression { get; set; }
        public long Id { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public string RegularExpression { get; set; }

        public long UserId { get; set; }

        public static ReadPlaceholderModel Convert(Placeholder placeholder)
        {
            return new ReadPlaceholderModel()
            {
                Id = placeholder.Id,
                Expression = placeholder.Expression,
                RegularExpression = placeholder.RegularExpression,
                CreationDate = placeholder.CreationDate,
                LastUpdateDate = placeholder.LastUpdateDate,
                UserId = placeholder.User?.Id ?? 0
            };
        }
    }

    public class UpdatePlaceholderModel
    {
        public string Expression { get; set; }
        public string RegularExpression { get; set; }
        public long UserId { get; set; }
    }
}