var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose")
                     .WithProperties(env =>
                     {
                         env.DashboardEnabled = true;
                     });

var umbraco = builder.AddProject<Projects.CMS_Umbraco>("cms-umbraco")
                     .PublishAsDockerComposeService((resource, service) =>
                     {
                         service.Name = "umbraco";
                     });

builder.Build().Run();
