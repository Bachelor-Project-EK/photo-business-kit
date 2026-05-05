using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");

var password = builder.AddParameter("db-password", secret: true);

var sql = builder.AddSqlServer("umbraco-db", password)
    .WithDockerfile(
        contextPath: "../Database")
    .WithDataVolume("umb_database")
    .WithEnvironment("MSSQL_SA_PASSWORD", password)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Healthcheck = new()
        {
            Test = [ "CMD", "bash", "/healthcheck.sh" ],
            Interval = "5m",
            Timeout = "5s",
            Retries = 3,
            StartPeriod = "30s"
        };
        service.Ports = ["1433:1433", "1434:1434"];
    });

builder.AddContainer("umbraco-cms", "umbraco.cms")
    .WithDockerfile(
        contextPath: "..",
        dockerfilePath: "CMS.Umbraco/Dockerfile")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080;https://+:8081")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetcore.pfx")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "DevOnlyPassword")
    .WithEnvironment("ConnectionStrings__umbracoDbDSN", sql.Resource.ConnectionStringExpression)
    .WithEnvironment("ConnectionStrings__umbracoDbDSN_ProviderName", "Microsoft.Data.SqlClient")
    .WithBindMount("../CMS.Umbraco/wwwroot/media", "/app/wwwroot/media")
    .WithBindMount("../CMS.Umbraco/wwwroot/scripts", "/app/wwwroot/scripts")
    .WithBindMount("../CMS.Umbraco/wwwroot/css", "/app/wwwroot/css")
    .WithBindMount("../CMS.Umbraco/Views", "/app/Views")
    .WithBindMount("../CMS.Umbraco/umbraco/models", "/app/umbraco/models")
    .WithVolume("umb_logs", "/app/umbraco/Logs")
    .WithVolume("umb_data", "/app/umbraco/Data")
    .WithHttpsEndpoint(targetPort: 8081)
    .WithExternalHttpEndpoints()
    .WaitFor(sql)
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
