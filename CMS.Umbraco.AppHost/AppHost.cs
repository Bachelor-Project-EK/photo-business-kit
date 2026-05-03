
/*docs
 * https://aspire.dev/deployment/docker-compose/#configure-a-container-registry
 * https://aspire.dev/dashboard/security-considerations/
 *
 */

using Aspire.Hosting.Docker;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("compose")
    .ConfigureEnvFile(env =>
    {
        env["DASHBOARD__OTLP__PRIMARYAPIKEY"] = new CapturedEnvironmentVariable
        {
            Name = "DASHBOARD__OTLP__PRIMARYAPIKEY",
            DefaultValue = "1234567890abcdef1234567890abcdef"
        };
    })
    .ConfigureComposeFile(compose =>
    {
        var dashboard = compose.Services["compose-dashboard"];
        dashboard.Environment["DASHBOARD__OTLP__AUTHMODE"] = "ApiKey";
        dashboard.Environment["DASHBOARD__OTLP__PRIMARYAPIKEY"] = "${DASHBOARD__OTLP__PRIMARYAPIKEY}";
    });


var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);
var umbracoDb = sql.AddDatabase("umbracoDbDSN");


builder.AddProject<Projects.CMS_Umbraco>("cms-umbraco")
    .WaitFor(umbracoDb)
    .WithReference(umbracoDb)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", "x-otlp-api-key=${DASHBOARD__OTLP__PRIMARYAPIKEY}")
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Name = "cms-umbraco";
        service.User = "root";
    });

builder.Build().Run();
