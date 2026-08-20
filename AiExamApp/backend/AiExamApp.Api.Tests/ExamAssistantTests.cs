using System.Net;
using System.Text;
using System.Text.Json;
using AiExamApp.Api.Models;
using AiExamApp.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AiExamApp.Api.Tests;

public sealed class ExamAssistantTests
{
    private static AiOptions TestOptions() => new()
    {
        BaseUrl = "https://zen.test/v1",
        DefaultModel = "deepseek-v4-flash-free",
        SystemPrompt = "Responde conciso.",
        Models =
        [
            new ModelDefinition { Id = "deepseek-v4-flash-free", Name = "DeepSeek V4 Flash Free", Free = true },
            new ModelDefinition { Id = "deepseek-v4-flash", Name = "DeepSeek V4 Flash", Free = false },
        ],
    };

    private static ExamAssistant CreateAssistant(FakeHandler handler, AiOptions? options = null)
    {
        var http = new HttpClient(handler);
        return new ExamAssistant(http, Options.Create(options ?? TestOptions()), NullLogger<ExamAssistant>.Instance);
    }

    [Fact]
    public async Task AskAsync_WithValidModel_ParsesContentFromZenResponse()
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """{"choices":[{"message":{"content":"Una interfaz define un contrato. Ejemplo: IDisposable."}}]}""",
                Encoding.UTF8, "application/json"),
        });

        var result = await CreateAssistant(handler).AskAsync("¿Qué es una interfaz?", "deepseek-v4-flash", CancellationToken.None);

        result.Answer.Should().Contain("contrato");
        result.Model.Should().Be("deepseek-v4-flash");
        result.ElapsedMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task AskAsync_NullModel_UsesDefault()
    {
        var handler = new FakeHandler(request =>
        {
            string body = request.Content!.ReadAsStringAsync().Result;
            body.Should().Contain("\"model\":\"deepseek-v4-flash-free\"");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"choices":[{"message":{"content":"ok"}}]}""", Encoding.UTF8, "application/json"),
            };
        });

        var result = await CreateAssistant(handler).AskAsync("pregunta", null, CancellationToken.None);
        result.Model.Should().Be("deepseek-v4-flash-free");
    }

    [Fact]
    public async Task AskAsync_UnknownModel_FallsBackToDefault()
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"choices":[{"message":{"content":"respuesta default"}}]}""", Encoding.UTF8, "application/json"),
        });

        var result = await CreateAssistant(handler).AskAsync("pregunta", "modelo-inexistente", CancellationToken.None);
        result.Model.Should().Be("deepseek-v4-flash-free");
    }

    [Fact]
    public async Task AskAsync_RequestedModelFails_FallsBackToDefault()
    {
        int calls = 0;
        var handler = new FakeHandler(_ =>
        {
            calls++;
            return calls == 1
                ? new HttpResponseMessage(HttpStatusCode.InternalServerError)
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"choices":[{"message":{"content":"fallback ok"}}]}""", Encoding.UTF8, "application/json"),
                };
        });

        var result = await CreateAssistant(handler).AskAsync("pregunta", "deepseek-v4-flash", CancellationToken.None);

        calls.Should().Be(2);
        result.Model.Should().Be("deepseek-v4-flash-free");
        result.Answer.Should().Be("fallback ok");
    }

    [Fact]
    public async Task AskAsync_DefaultFreeRateLimited_RetriesNextFreeModel()
    {
        var options = TestOptions();
        options.Models =
        [
            new ModelDefinition { Id = "deepseek-v4-flash-free", Name = "DeepSeek Free", Free = true },
            new ModelDefinition { Id = "hy3-free", Name = "Hy3 Free", Free = true },
        ];

        int calls = 0;
        string[] modelsSeen = [];
        var handler = new FakeHandler(request =>
        {
            string body = request.Content!.ReadAsStringAsync().Result;
            modelsSeen = [.. modelsSeen, body.Contains("\"model\":\"deepseek-v4-flash-free\"") ? "deepseek-v4-flash-free" : "hy3-free"];
            calls++;
            return calls == 1
                ? new HttpResponseMessage((HttpStatusCode)429)
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"choices":[{"message":{"content":"respuesta de hy3"}}]}""", Encoding.UTF8, "application/json"),
                };
        });

        var result = await CreateAssistant(handler, options).AskAsync("pregunta", null, CancellationToken.None);

        calls.Should().Be(2);
        modelsSeen.Should().Equal(["deepseek-v4-flash-free", "hy3-free"]);
        result.Model.Should().Be("hy3-free");
    }

    [Fact]
    public void GetModels_ReturnsDefaultAndCatalog()
    {
        var models = CreateAssistant(new FakeHandler(_ => new HttpResponseMessage())).GetModels();

        models.Default.Should().Be("deepseek-v4-flash-free");
        models.Models.Should().HaveCount(2);
        models.Models[0].Free.Should().BeTrue();
    }

    [Fact]
    public async Task AskAsync_Payload_ContainsSystemPromptAndQuestion()
    {
        string? capturedBody = null;
        var handler = new FakeHandler(request =>
        {
            capturedBody = request.Content!.ReadAsStringAsync().Result;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"choices":[{"message":{"content":"ok"}}]}""", Encoding.UTF8, "application/json"),
            };
        });

        await CreateAssistant(handler).AskAsync("mi pregunta", null, CancellationToken.None);

        using var doc = JsonDocument.Parse(capturedBody!);
        doc.RootElement.GetProperty("messages")[0].GetProperty("content").GetString().Should().Be("Responde conciso.");
        doc.RootElement.GetProperty("messages")[1].GetProperty("content").GetString().Should().Be("mi pregunta");
    }

    private sealed class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
            Task.FromResult(respond(request));
    }
}