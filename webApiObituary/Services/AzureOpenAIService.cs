using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MemorialRegistry.Shared.DTOs;

namespace Assignment1.Services;

public class AzureOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly AzureOpenAIOptions _options;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(HttpClient httpClient, AzureOpenAIOptions options, ILogger<AzureOpenAIService> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<GenerateBiographyResponse> GenerateBiographyAsync(GenerateBiographyRequest request)
    {
        try
        {
            var fullUrl = $"{_options.Endpoint}?api-version={_options.ApiVersion}";

            var prompt = $@"Help me create a full, detailed obituary biography based on this information:

Full Name: {request.FullName}
Date of Birth: {request.DateOfBirth:MMMM d, yyyy}
Date of Death: {request.DateOfDeath:MMMM d, yyyy}
Key Points: {request.Biography}

Please expand these points into a respectful ~200 word obituary biography.";

            var payload = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_completion_tokens = _options.MaxTokens,
                model = _options.Model
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(fullUrl, content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Azure OpenAI Error {Status}: {Body}", response.StatusCode, body);

                return new GenerateBiographyResponse
                {
                    Success = false,
                    ErrorMessage = $"Azure OpenAI error: {response.StatusCode}"
                };
            }

            var result = JsonSerializer.Deserialize<JsonElement>(body);

            var generated = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new GenerateBiographyResponse
            {
                Success = true,
                GeneratedBiography = generated ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI request failure");

            return new GenerateBiographyResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
