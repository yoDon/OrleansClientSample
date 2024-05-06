using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.AddKeyedRedisClient("orleans-redis", null, options =>
{
    options.AsyncTimeout = 30_000;
    options.SyncTimeout = 30_000;
});

builder.UseOrleans(siloBuilder =>
{
    siloBuilder
        .AddActivityPropagation()
        .ConfigureLogging(logging => logging.AddConsole());
    
    if (builder.Environment.IsDevelopment())
    {
        siloBuilder
            .ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
    }
});

var app = builder.Build();

app.MapGet("/", () => "Silo")
    .WithOpenApi();

app.UseSwagger();
app.UseSwaggerUI();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.Run();
