
using Microsoft.Data.SqlClient;
using OpenTelemetry.Exporter;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();

await EnsureSqlServerDatabaseExistsAsync(builder.Configuration, "umbracoDbDSN");

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

// builder.AddSqlServerClient(connectionName: "umbracoDbDSN");

// builder.Services.Configure<OtlpExporterOptions>(o =>
//     o.Headers = $"x-otlp-api-key={builder.Configuration["MY_APIKEY"]}");

WebApplication app = builder.Build();

app.Logger.LogInformation("Env: {env}", app.Environment.EnvironmentName);

app.MapDefaultEndpoints();


await app.BootUmbracoAsync();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();

static async Task EnsureSqlServerDatabaseExistsAsync(IConfiguration configuration, string connectionName)
{
    var connectionString = configuration.GetConnectionString(connectionName);

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return;
    }

    var databaseConnectionString = new SqlConnectionStringBuilder(connectionString);
    var databaseName = databaseConnectionString.InitialCatalog;

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        return;
    }

    var masterConnectionString = new SqlConnectionStringBuilder(databaseConnectionString.ConnectionString)
    {
        InitialCatalog = "master"
    };

    for (var attempt = 1; attempt <= 30; attempt++)
    {
        try
        {
            await using var connection = new SqlConnection(masterConnectionString.ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = $"""
                IF DB_ID(N'{databaseName.Replace("'", "''")}') IS NULL
                BEGIN
                    CREATE DATABASE {QuoteSqlIdentifier(databaseName)}
                END
                """;

            await command.ExecuteNonQueryAsync();
            return;
        }
        catch (SqlException) when (attempt < 30)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

static string QuoteSqlIdentifier(string value)
    => $"[{value.Replace("]", "]]")}]";
