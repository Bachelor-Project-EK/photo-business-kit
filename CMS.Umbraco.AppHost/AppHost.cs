var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CMS_Umbraco>("cms-umbraco");

builder.Build().Run();
