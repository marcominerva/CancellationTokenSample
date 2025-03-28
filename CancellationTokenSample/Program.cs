using Microsoft.AspNetCore.Http.Timeouts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRequestTimeouts();

builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    _ = options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseRouting();

app.UseRequestTimeouts();

app.MapPost("/api/cancellation-token", async (CancellationToken cancellationToken, int delay = 5000) =>
{
    await Task.Delay(delay, cancellationToken);

    return TypedResults.NoContent();
})
.WithRequestTimeout(new RequestTimeoutPolicy
{
    Timeout = TimeSpan.FromSeconds(2),
    TimeoutStatusCode = StatusCodes.Status408RequestTimeout
})
.WithDescription("Remember to test with endpoint without the debugger if you want to verify the Request Timeout (2 seconds)");

app.Run();