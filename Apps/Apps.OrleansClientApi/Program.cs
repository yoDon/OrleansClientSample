using Grains.GrainDef;

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
