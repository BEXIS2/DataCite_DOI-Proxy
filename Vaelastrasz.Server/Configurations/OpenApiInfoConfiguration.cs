using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;

namespace Vaelastrasz.Server.Configurations
{
    public class OpenApiInfoConfiguration
    {
        public required string Title { get; set; }
        public required string Description { get; set; }

        [Url]
        public string? TermsOfService { get; set; }

        public required OpenApiContactConfiguration Contact { get; set; }

        public required OpenApiLicenseConfiguration License { get; set; }

        public OpenApiInfo GetOpenApiInfo()
        {
            if (Validator.TryValidateObject(this, new ValidationContext(this), null, validateAllProperties: true))
            {
                return new OpenApiInfo
                {
                    Title = this.Title,
                    Description = this.Description,
                    TermsOfService = string.IsNullOrWhiteSpace(this.TermsOfService) ? null : new Uri(this.TermsOfService),
                    Contact = this.Contact == null ? null : new OpenApiContact
                    {
                        Name = this.Contact.Name,
                        Email = this.Contact.Email,
                        Url = string.IsNullOrWhiteSpace(this.Contact.Url) ? null : new Uri(this.Contact.Url)
                    },
                    License = this.License == null ? null : new OpenApiLicense
                    {
                        Name = this.License.Name,
                        Url = string.IsNullOrWhiteSpace(this.License.Url) ? null : new Uri(this.License.Url)
                    }
                };
            }

            return new OpenApiInfo();
        }
    }

    public class OpenApiContactConfiguration
    {
        public required string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Url]
        public string? Url { get; set; }
    }

    public class OpenApiLicenseConfiguration
    {
        public required string Name { get; set; }

        [Url]
        public string? Url { get; set; }
    }
}