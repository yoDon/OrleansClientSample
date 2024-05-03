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

builder.AddKeyedRedisClient("orleans-redis", null, options =>
{
    options.AsyncTimeout = 30_000;
    options.SyncTimeout = 30_000;
});

builder.UseOrleansClient();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.MapGet("/sample/hello/{helloId}", 
        async (IClusterClient orleansClient, HttpResponse response, HttpContext context, string helloId) =>
        {
            var grain = IHelloGrain.GetGrain(orleansClient, helloId);
            var result = await grain.SayHello();
            return Results.Ok(result);
        })
    .WithOpenApi();

app.MapGet("/", () =>
        "This is a web application that talks to Orleans."
    )
    .WithOpenApi();

app.MapDefaultEndpoints();
app.Run();
