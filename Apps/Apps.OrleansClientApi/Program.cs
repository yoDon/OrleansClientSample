using System.Collections;
using Grains.GrainDef;

var useOrleansClientHack = true;
if (useOrleansClientHack)
{
    // Hack to get around issue where UseOrleansClient thinks it needs to configure grain state providers...
    var environmentVariables = Environment.GetEnvironmentVariables();
    foreach (DictionaryEntry dictionaryEntry in environmentVariables)
    {
        Console.WriteLine($"---- {dictionaryEntry.Key}");
        if (dictionaryEntry.Key.ToString()!.StartsWith("Orleans__GrainStorage__"))
        {
            Console.WriteLine("FOUND IT - CLEARING IT");
            Environment.SetEnvironmentVariable(dictionaryEntry.Key.ToString()!, null);
        }
    }
}

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

builder.UseOrleansClient();

var app = builder.Build();

app.MapGet("/sample/hello/{helloId}", 
        async (IClusterClient orleansClient, string helloId) =>
        {
            var grain = IHelloGrain.GetGrain(orleansClient, helloId);
            var result = await grain.SayHello();
            return Results.Ok(result);
        })
    .WithOpenApi();

app.MapGet("/", () => "This is a web application that talks to Orleans.")
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
