var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Assignment1>("backend");

// builder.AddProject<Projects.Assignment1>("frontend")
//     .WithReference(api)
//     .WaitFor(api);

builder.Build().Run();
