using AiExamApp.Api.Models;

namespace AiExamApp.Api.Services;

public interface IExamAssistant
{
    ModelsResponse GetModels();
    Task<AskResponse> AskAsync(string question, string? requestedModel, CancellationToken ct);
}