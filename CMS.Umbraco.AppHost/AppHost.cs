using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("db-password", secret: true);
var metadataAddress = builder.AddParameter("metadata-address", secret: true);
var clientId = builder.AddParameter("client-id", secret: true);
var clientSecret = builder.AddParameter("client-secret", secret: true);
var logoutUrl = builder.AddParameter("logout-url", secret: true);

builder.AddDockerComposeEnvironment("env");

var storage = builder.AddAzureStorage("umbraco-blob-storage")
    .RunAsEmulator(azurite =>
    {
        azurite.WithDataVolume("umb_storage");
    })
    .AddBlobs("umbraco-media");

var sql = builder.AddAzureSqlServer("umbraco-sql-server")
    .RunAsContainer(options =>
    {
        options.WithHostPort(1533);
        options.WithPassword(password);
        options.WithDockerfile("../Database");
        options.WithDataVolume("umb_database");
        options.WithEnvironment("MSSQL_SA_PASSWORD", password);
    });

var db = sql.AddDatabase("UmbracoDb");

var cmsPort = 44353;

builder.AddDockerfile(name: "umbraco-cms", contextPath: "..", dockerfilePath: "CMS.Umbraco/Dockerfile")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080/;https://+:8081/")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetcore.pfx")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "DevOnlyPassword")
    .WithEnvironment("ConnectionStrings__umbracoDbDSN_ProviderName", "Microsoft.Data.SqlClient")
    .WithEnvironment("Umbraco__CMS__Webrouting__UmbracoApplicationUrl", "https+http://umbraco-cms")
    .WithEnvironment("OpenIdConnect__MetadataAddress", metadataAddress)
    .WithEnvironment("OpenIdConnect__ClientId", clientId)
    .WithEnvironment("OpenIdConnect__ClientSecret", clientSecret)
    .WithEnvironment("OpenIdConnect__LogoutUrl", logoutUrl)
    .WithEnvironment("OpenIdConnect__ReturnAfterLogout", "https+http://umbraco-cms")
    .WithBindMount("../CMS.Umbraco/wwwroot/media", "/app/wwwroot/media")
    .WithBindMount("../CMS.Umbraco/wwwroot/scripts", "/app/wwwroot/scripts")
    .WithBindMount("../CMS.Umbraco/wwwroot/css", "/app/wwwroot/css")
    .WithBindMount("../CMS.Umbraco/Views", "/app/Views")
    .WithBindMount("../CMS.Umbraco/umbraco/models", "/app/umbraco/models")
    .WithBindMount("../CMS.Umbraco/uSync", "/app/uSync")
    .WithVolume("umb_logs", "/app/umbraco/Logs")
    .WithVolume("umb_data", "/app/umbraco")
    .WithHttpsEndpoint(cmsPort, 8081)
    .WithExternalHttpEndpoints()
    .WithReference(db, "umbracoDbDSN")
    .WaitFor(db)
    .WithReference(storage, "umbracoBlobStorage")
    .WaitFor(storage)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.DependsOn = new()
        {
            { "umbraco-db", new() { Condition = "service_healthy" } }
        };
        service.Healthcheck = new()
        {
            Test =
            [
                "CMD",
                "curl",
                "-f",
                ("https://+8081:/umbraco/api/health/ready")
            ],
            Interval = "10s",
            Timeout = "5s",
            Retries = 3,
            StartPeriod = "30s"
        };
    });

await builder.Build().RunAsync();
