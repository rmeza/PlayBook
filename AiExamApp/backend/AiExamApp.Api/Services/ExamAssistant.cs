using System.Diagnostics;
using System.Text;
using System.Text.Json;
using AiExamApp.Api.Models;
using Microsoft.Extensions.Options;

namespace AiExamApp.Api.Services;

public sealed class ExamAssistant : IExamAssistant
{
    private readonly HttpClient _http;
    private readonly AiOptions _options;
    private readonly ILogger<ExamAssistant> _logger;

    public ExamAssistant(HttpClient http, IOptions<AiOptions> options, ILogger<ExamAssistant> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public ModelsResponse GetModels() =>
        new(_options.DefaultModel, _options.Models);

    public async Task<AskResponse> AskAsync(string question, string? requestedModel, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        string? lastError = null;

        foreach (string model in FallbackChain(requestedModel))
        {
            try
            {
                string answer = await SendChatAsync(question, model, ct);
                sw.Stop();
                return new AskResponse(answer, model, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                _logger.LogWarning(ex, "Model {Model} failed, trying next in chain", model);
            }
        }

        sw.Stop();
        throw new HttpRequestException(lastError ?? "No model available");
    }

    private List<string> FallbackChain(string? requested)
    {
        string primary = ResolveModel(requested);
        var chain = new List<string> { primary };
        chain.AddRange(_options.Models
            .Where(m => m.Free && m.Id != primary)
            .Select(m => m.Id));
        return chain.Distinct().ToList();
    }

    private string ResolveModel(string? requested)
    {
        if (string.IsNullOrWhiteSpace(requested)) return _options.DefaultModel;
        bool known = _options.Models.Any(m => m.Id == requested);
        return known ? requested : _options.DefaultModel;
    }

    private async Task<string> SendChatAsync(string question, string model, CancellationToken ct)
    {
        var payload = new
        {
            model,
            messages = new object[]
            {
                new { role = "system", content = _options.SystemPrompt },
                new { role = "user", content = question },
            },
            temperature = 0.2,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
        };

        using var response = await _http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Zen returned {(int)response.StatusCode}: {body[..Math.Min(body.Length, 300)]}");
        }

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
    }
}