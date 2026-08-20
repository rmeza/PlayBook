namespace AiExamApp.Api.Models;

public sealed record AskRequest(string Question, string? Model = null);

public sealed record AskResponse(string Answer, string Model, long ElapsedMs);

public sealed record ModelsResponse(string Default, IReadOnlyList<ModelDefinition> Models);

public sealed record HealthResponse(string Status);