var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("db-password", secret: true);
var metadataAddress = builder.AddParameter("metadata-address", secret: true);
var clientId = builder.AddParameter("client-id", secret: true);
var clientSecret = builder.AddParameter("client-secret", secret: true);
var logoutUrl = builder.AddParameter("logout-url", secret: true);
if (!builder.ExecutionContext.IsPublishMode)
{
    builder.AddDockerComposeEnvironment("env");
}

var storage = builder.AddAzureStorage("umbraco-blob-storage");

if (!builder.ExecutionContext.IsPublishMode)
{
    storage.RunAsEmulator(azurite =>
    {
        azurite.WithDataVolume("umb_storage");
        azurite.WithBlobPort(27000)
            .WithQueuePort(27001)
            .WithTablePort(27002);
    });
}

var blobs = storage.AddBlobs("umbraco-media");

var sql = builder.AddAzureSqlServer("umbraco-sql-server");

if (!builder.ExecutionContext.IsPublishMode)
{
    sql.RunAsContainer(options =>
    {
        options.WithHostPort(1533);
        options.WithPassword(password);
        options.WithDockerfile("../Database");
        options.WithDataVolume("umb_database");
        options.WithEnvironment("MSSQL_SA_PASSWORD", password);
    });
}


var db = sql.AddDatabase("UmbracoDb");

var cmsPort = 44353;


var cms = builder.AddDockerfile(
        name: "umbraco-cms",
        contextPath: "..",
        dockerfilePath: "CMS.Umbraco/Dockerfile")
    .WithEnvironment("OpenIdConnect__MetadataAddress", metadataAddress)
    .WithEnvironment("OpenIdConnect__ClientId", clientId)
    .WithEnvironment("OpenIdConnect__ClientSecret", clientSecret)
    .WithEnvironment("OpenIdConnect__LogoutUrl", logoutUrl)
    .WithEnvironment("ConnectionStrings__umbracoDbDSN_ProviderName", "Microsoft.Data.SqlClient")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
    .WithEnvironment("Umbraco__CMS__Global__MainDomLock", "FileSystemMainDomLock")
    .WithEnvironment("Umbraco__CMS__Hosting__LocalTempStorageLocation", "EnvironmentTemp")
    .WithEnvironment("Umbraco__CMS__Examine__LuceneDirectoryFactory", "TempFileSystemDirectoryFactory")
    .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ConnectionString", blobs)
    .WithEnvironment("Umbraco__Storage__AzureBlob__Media__ContainerName", "umbraco-media")
    .WithEnvironment("ASPNETCORE_FORWARDEDHEADERS_ENABLED", "true")
    .WithHttpEndpoint(targetPort: 8080, name: "http")
    .WithExternalHttpEndpoints()
    .WithReference(db, "umbracoDbDSN")
    .WaitFor(db)
    .WithReference(blobs, "umbracoBlobStorage")
    .WaitFor(blobs);

if (!builder.ExecutionContext.IsPublishMode)
{
    cms
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        .WithEnvironment("ASPNETCORE_URLS", "http://+:8080/;https://+:8081/")
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetcore.pfx")
        .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "DevOnlyPassword")
        .WithEnvironment("ConnectionStrings__umbracoDbDSN_ProviderName", "Microsoft.Data.SqlClient")
        .WithEnvironment("Umbraco__CMS__Webrouting__UmbracoApplicationUrl", "https+http://umbraco-cms")
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
                    "https://localhost:8081/umbraco/api/health/ready"
                ],
                Interval = "10s",
                Timeout = "5s",
                Retries = 3,
                StartPeriod = "30s"
            };
        });
}


await builder.Build().RunAsync();
