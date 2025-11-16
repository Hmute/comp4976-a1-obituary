using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MemorialRegistry.Shared.DTOs;

namespace Assignment1.Services;

public class AzureOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<AzureOpenAIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GenerateBiographyResponse> GenerateBiographyAsync(GenerateBiographyRequest request)
    {
        try
        {
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var apiVersion = _configuration["AzureOpenAI:ApiVersion"];
            var model = _configuration["AzureOpenAI:Model"];
            var maxTokens = int.Parse(_configuration["AzureOpenAI:MaxTokens"] ?? "40000");

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Azure OpenAI configuration is missing");
                return new GenerateBiographyResponse
                {
                    Success = false,
                    ErrorMessage = "Azure OpenAI service is not configured"
                };
            }

            var fullUrl = $"{endpoint}?api-version={apiVersion}";

            var prompt = $@"Help me create a full, detailed obituary biography based on this information:

Full Name: {request.FullName}
Date of Birth: {request.DateOfBirth:MMMM d, yyyy}
Date of Death: {request.DateOfDeath:MMMM d, yyyy}
Key Points: {request.Biography}

Please expand these key points into a complete, touching, and respectful biography that honors their life and legacy.
The result should be within 200 words.";

            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_completion_tokens = maxTokens,
                model = model
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(fullUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Azure OpenAI API error: {response.StatusCode} - {responseContent}");
                return new GenerateBiographyResponse
                {
                    Success = false,
                    ErrorMessage = $"API error: {response.StatusCode}"
                };
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var generatedText = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new GenerateBiographyResponse
            {
                Success = true,
                GeneratedBiography = generatedText ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating biography with Azure OpenAI");
            return new GenerateBiographyResponse
            {
                Success = false,
                ErrorMessage = $"An error occurred: {ex.Message}"
            };
        }
    }
}
