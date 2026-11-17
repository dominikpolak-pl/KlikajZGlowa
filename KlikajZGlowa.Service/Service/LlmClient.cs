using KlikajZGlowa.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace KlikajZGlowa.Service.Service;

public sealed class LlmClient : ILlmClient
{
    private const string InteractionsEndpoint =
        "https://generativelanguage.googleapis.com/v1beta/interactions";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly ILogger<LlmClient> _logger;

    public LlmClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LlmClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _apiKey = configuration["AiSettings:ApiKey"]
            ?? throw new InvalidOperationException(
                "Brak klucza Gemini API w konfiguracji.");

        _modelName = configuration["AiSettings:Model"]
            ?? "gemini-3.8-flash";
    }

    public async Task<string> GetStructuredCompletionAsync(
        string systemPrompt,
        string userPrompt)
    {
        var requestBody = new
        {
            model = _modelName,
            system_instruction = systemPrompt,
            input = userPrompt
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            InteractionsEndpoint);

        request.Headers.Add("x-goog-api-key", _apiKey);
        request.Headers.Add("Api-Revision", "2026-05-20");
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        _logger.LogInformation(
            "Wysyłanie żądania do Gemini. Model: {Model}",
            _modelName);

        using var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation(
            "Gemini odpowiedziało kodem {StatusCode}: {ResponseBody}",
            (int)response.StatusCode,
            responseBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Gemini API zwróciło błąd {(int)response.StatusCode}: " +
                responseBody);
        }

        return ExtractTextFromResponse(responseBody);
    }

    private static string ExtractTextFromResponse(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        if (!root.TryGetProperty("steps", out var steps))
        {
            throw new InvalidOperationException(
                "Gemini nie zwróciło kolekcji steps.");
        }

        foreach (var step in steps.EnumerateArray())
        {
            if (!step.TryGetProperty("type", out var type) ||
                type.GetString() != "model_output")
            {
                continue;
            }

            if (!step.TryGetProperty("content", out var content))
            {
                continue;
            }

            foreach (var item in content.EnumerateArray())
            {
                if (!item.TryGetProperty("type", out var itemType) ||
                    itemType.GetString() != "text")
                {
                    continue;
                }

                if (item.TryGetProperty("text", out var text))
                {
                    var result = text.GetString();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                }
            }
        }

        throw new InvalidOperationException(
            "Gemini zwróciło pustą odpowiedź tekstową.");
    }
}