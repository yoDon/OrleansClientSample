var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("orleans-redis");
var orleans = builder.AddOrleans("orleans-cluster")
    .WithClustering(redis);

var silo =
    builder.AddProject<Projects.Apps_Silo>("silo")
        .WithReference(orleans)
        .WithReplicas(3);

var clientApi =
    builder.AddProject<Projects.Apps_OrleansClientApi>("orleansclientapi")
        .WithReference(orleans)
        .WithExternalHttpEndpoints();

var weatherApi =
    builder.AddProject<Projects.Apps_NonClientApi>("nonclientapi")
        .WithExternalHttpEndpoints();

builder.Build().Run();
