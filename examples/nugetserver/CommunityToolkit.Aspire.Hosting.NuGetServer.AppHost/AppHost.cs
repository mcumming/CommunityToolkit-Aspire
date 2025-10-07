var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddNuGetServer("NuGetServer");

builder.Build().Run();
