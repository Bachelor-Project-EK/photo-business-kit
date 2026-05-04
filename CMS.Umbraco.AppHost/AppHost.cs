using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env")
    .WithDashboard(enabled: true)
    .ConfigureComposeFile(source =>
    {
        source.AddVolume(new()
        {
            Name = "umb_database",
        });
        source.AddVolume(new()
        {
            Name = "umb_data",
        });
        source.AddVolume(new()
        {
            Name = "umb_logs",
        });
    });

var password = builder.AddParameter("db-password", secret: true);

var sql = builder.AddSqlServer("umbraco-db", password)
    .WithDockerfile(
        contextPath: "../Database")
    .WithDataVolume("umb_database")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", password)
    .WithEnvironment("MSSQL_SA_PASSWORD", password)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "umbraco-db";
        service.Healthcheck = new()
        {
            Test =
            [
                "CMD",
                "bash",
                "/healthcheck.sh"
            ],
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
    .WithHttpsEndpoint(44372, 8081)
    .WithExternalHttpEndpoints()
    .WaitFor(sql)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Name = "umbraco-cms";
        service.Ports = ["44372:8081"];
        service.Volumes = new()
        {
            new()
            {
                Name = "media",
                Type = "bind",
                Source = "../CMS.Umbraco/wwwroot/media",
                Target = "/app/wwwroot/media"
            },
            new()
            {
                Name = "scripts",
                Type = "bind",
                Source = "../CMS.Umbraco/wwwroot/scripts",
                Target = "/app/wwwroot/scripts"
            },
            new()
            {
                Name = "css",
                Type = "bind",
                Source = "../CMS.Umbraco/wwwroot/css",
                Target = "/app/wwwroot/css"
            },
            new()
            {
                Name = "Views",
                Type = "bind",
                Source = "../CMS.Umbraco/wwwroot/views",
                Target = "/app/wwwroot/views"
            },
            new()
            {
                Name = "models",
                Type = "bind",
                Source = "../CMS.Umbraco/wwwroot/models",
                Target = "/app/wwwroot/models"
            },
            new()
            {
                Name = "umb_data",
                Type = "volume",
                Source = "umb_data",
                Target = "/app_data"
            },
            new()
            {
                Name = "umb_logs",
                Type = "volume",
                Source = "umb_logs",
                Target = "/logs"
            }
        };
        service.DependsOn = new()
        {
            { "umbraco-db", new() { Condition = "service_healthy" } }
        };
    });

await builder.Build().RunAsync();
