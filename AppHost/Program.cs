var builder = DistributedApplication.CreateBuilder(args);

var redis =
    builder.AddRedis("orleans-redis")
        .WithHealthCheck();

var orleans = 
    builder.AddOrleans("orleans-cluster")
        .WithClustering(redis);

var silo =
    builder.AddProject<Projects.Apps_Silo>("silo")
        .WithReference(orleans)
        .WaitOn(redis)
        .WithReplicas(3);

var clientApi =
    builder.AddProject<Projects.Apps_OrleansClientApi>("orleans-client-api")
        .WithReference(orleans)
        .WithExternalHttpEndpoints();

var nonOrleansApi =
    builder.AddProject<Projects.Apps_NonClientApi>("non-orleans-api")
        .WithExternalHttpEndpoints();

builder.Build().Run();
