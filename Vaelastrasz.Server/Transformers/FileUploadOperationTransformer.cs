using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using Vaelastrasz.Server.Attributes;

namespace Vaelastrasz.Server.Transformers
{
    public sealed class FileUploadOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var endpointMetadata = context.Description.ActionDescriptor.EndpointMetadata;

            // z.B. dein Attribut so finden:
            var attr = endpointMetadata
                 .OfType<SwaggerCustomHeaderAttribute>()
                 .FirstOrDefault();

            if (attr != null)
            {
                // Hier machen, was dein alter Filter gemacht hat
                var mediaTypes = attr.AcceptableTypes
                    .Distinct()
                    .Select(t => (JsonNode)JsonValue.Create(t))
                    .ToList();

                operation.Parameters ??= new List<IOpenApiParameter>();
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = attr.HeaderName,
                    In = ParameterLocation.Header,
                    Description = "Specifies the acceptable media type.",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Enum = mediaTypes
                    }
                });
            }

            // Multipart-Form / Datei-Upload wie gehabt
            if (operation.RequestBody?.Content?.ContainsKey("multipart/form-data") == true)
            {
                var mediaType = operation.RequestBody.Content["multipart/form-data"];

                var schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["file"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Format = "binary"
                        }
                    }
                };

                // ✅ Erstelle ein neues OpenApiMediaType mit dem neuen Schema
                var newMediaType = new OpenApiMediaType
                {
                    Schema = schema
                    // Optional: Kopiere andere Eigenschaften wie Examples, Encoding, etc.
                };

                // ✅ Ersetze das alte MediaType im Dictionary
                operation.RequestBody.Content["multipart/form-data"] = newMediaType;
            }

            return Task.CompletedTask;
        }
    }
}