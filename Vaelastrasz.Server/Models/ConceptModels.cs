using System.ComponentModel.DataAnnotations;

namespace Vaelastrasz.Server.Models
{
    public class CreateConceptModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        [Url]
        public required string Url { get; set; }

        public List<CreateConceptItemModel> Items { get; set; }

        public CreateConceptModel()
        {
            Items = [];
        }
    }

    public class CreateConceptItemModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        [Url]
        public required string Url { get; set; }

        public bool IsOptional { get; set; }
        public bool IsComplex { get; set; }
        public required string XPath { get; set; }

        public List<CreateConceptItemModel> Children { get; set; }

        public CreateConceptItemModel() => Children = [];
    }
}