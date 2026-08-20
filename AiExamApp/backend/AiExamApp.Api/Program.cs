using AiExamApp.Api.Models;
using AiExamApp.Api.Services;
using Microsoft.Extensions.Options;

LoadEnvFile(Path.Combine(AppContext.BaseDirectory, ".env"));
LoadEnvFile(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

static void LoadEnvFile(string path)
{
    if (!File.Exists(path)) return;
    foreach (string rawLine in File.ReadAllLines(path))
    {
        string line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith('#')) continue;
        int eq = line.IndexOf('=');
        if (eq <= 0) continue;
        string key = line[..eq].Trim();
        string value = line[(eq + 1)..].Trim().Trim('"');
        if (Environment.GetEnvironmentVariable(key) is null)
            Environment.SetEnvironmentVariable(key, value);
    }
}

builder.Services.AddOptions<AiOptions>()
    .Bind(builder.Configuration.GetSection(AiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<IExamAssistant, ExamAssistant>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    string? apiKey = Environment.GetEnvironmentVariable("ZEN_API_KEY");
    if (!string.IsNullOrEmpty(apiKey))
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new HealthResponse("ok")));

app.MapGet("/api", (IExamAssistant assistant) => Results.Ok(new
{
    app = "AiExamApp API",
    hint = "Esta es la API del backend. La app web vive en http://localhost:4200. Usa /api/models, /api/ask o /api/health.",
    models = assistant.GetModels().Models.Count,
}));

app.MapGet("/api/models", (IExamAssistant assistant) => Results.Ok(assistant.GetModels()));

app.MapPost("/api/ask", async (AskRequest request, IExamAssistant assistant, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Question))
        return Results.BadRequest(new { error = "question is required" });

    try
    {
        var result = await assistant.AskAsync(request.Question, request.Model, ct);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Ask failed: {Model}", request.Model);
        return Results.Json(new { error = "El proveedor de IA no respondió. Inténtalo de nuevo." }, statusCode: StatusCodes.Status502BadGateway);
    }
});

app.Run();

public partial class Program;