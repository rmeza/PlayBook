namespace AiExamApp.Api.Models;

public sealed class AiOptions
{
    public const string SectionName = "Ai";

    public string BaseUrl { get; set; } = "https://opencode.ai/zen/v1";
    public string DefaultModel { get; set; } = "deepseek-v4-flash-free";
    public string SystemPrompt { get; set; } = "";
    public double TimeoutSeconds { get; set; } = 60;
    public List<ModelDefinition> Models { get; set; } = [];
}

public sealed class ModelDefinition
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool Free { get; set; }
}