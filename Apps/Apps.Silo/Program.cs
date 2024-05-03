using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyedRedisClient("orleans-redis", null, options =>
{
    options.AsyncTimeout = 30_000;
    options.SyncTimeout = 30_000;
});

builder.UseOrleans(siloBuilder =>
{
    siloBuilder
        .Configure<SiloOptions>(options => { options.SiloName = "Silo"; })
        .AddActivityPropagation()
        .ConfigureLogging(logging => logging.AddConsole())
        ;
    
    siloBuilder
        .UseDashboard(config =>
            config.HideTrace =
                string.IsNullOrEmpty(builder.Configuration.GetValue<string>("HideTrace"))
                || builder.Configuration.GetValue<bool>("HideTrace"))
        ;
    
    if (builder.Environment.IsDevelopment())
    {
        siloBuilder
            .ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
    }
});

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => Results.Ok("Silo"));

app.Run();
