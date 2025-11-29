using Exceptionless;
using LiteDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Vaelastrasz.Library.Resolvers;
using Vaelastrasz.Server.Attributes;
using Vaelastrasz.Server.Authentication;
using Vaelastrasz.Server.Configurations;
using Vaelastrasz.Server.Filters;
using Vaelastrasz.Server.Middleware;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
.Build();

var openApiInfoConfiguration = configuration.GetSection("OpenApiInfo").Get<OpenApiInfoConfiguration>() ?? new OpenApiInfoConfiguration
{
    Title = "DataCite DOI-Proxy",
    Description = "A proxy service, not only, but specifically for BEXIS2 instances to communicate with DataCite.",
    Contact = new OpenApiContactConfiguration
    {
        Name = "Sven Thiel",
        Email = "m6thsv2@googlemail.com",
        Url = "https://github.com/sventhiel/Vaelastrasz"
    },
    License = new OpenApiLicenseConfiguration
    {
        Name = "GPL-3.0 license",
        Url = "https://www.gnu.org/licenses/gpl-3.0.html#license-text"
    }
};

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// LiteDB
builder.Services.AddSingleton(new ConnectionString(builder.Configuration["ConnectionStrings:Vaelastrasz"]));

// Exceptionless
builder.Services.AddExceptionless(options =>
{
    options.ServerUrl = builder.Configuration["Exceptionless:ServerUrl"] ?? "";
    options.ApiKey = builder.Configuration["Exceptionless:ApiKey"] ?? "";
    options.IncludeUserName = Convert.ToBoolean(builder.Configuration["Exceptionless:IncludeUserName"]);
    options.IncludeIpAddress = Convert.ToBoolean(builder.Configuration["Exceptionless:IncludeIpAddress"]);
    options.SetVersion($"{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}");
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalEnumValidationFilter>();
}).AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new VaelastraszContractResolver();
    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    //o.SerializerSettings.Converters.Add(new StringEnumConverter());
}).AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
});

builder.Services.AddAuthentication("basicAuth").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("basicAuth", null);
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = openApiInfoConfiguration.GetOpenApiInfo();

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Basic Auth"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            Description = "Basic Authentication"
        };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, ct) =>
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
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()  // ← WICHTIG
            };

            schema.Properties["file"] = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Format = "binary"
            };

            operation.RequestBody.Content["multipart/form-data"].Schema = schema;
        }

        return Task.CompletedTask;
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost;

    // Falls Proxy private IP hat:
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (Convert.ToBoolean(builder.Configuration["Exceptionless:Enabled"]))
{
    app.UseExceptionless();
    ExceptionlessClient.Default.CreateLog("Vaelastrasz.Server started").Submit();
}

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference("/", options =>
{
    options.Title = "DataCite DOI Proxy";
    //options.Theme = ScalarTheme.Mars;
    options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.RestSharp);
    options.HideModels = false;
    options.Layout = ScalarLayout.Classic;
    options.ShowSidebar = true;
    if (Uri.TryCreate(builder.Configuration["Scalar:Server"], UriKind.Absolute, out var uri))
        options.AddServer(new ScalarServer(uri.AbsoluteUri));
}).AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseForwardedHeaders();

app.Run();