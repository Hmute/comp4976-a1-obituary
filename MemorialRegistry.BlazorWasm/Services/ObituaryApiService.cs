/* 
 * OBITUARY API SERVICE
 * ===================
 * 
 * 📡 MIGRATION: Entity Framework Direct Access → HTTP API Client
 * 
 * This service replaces direct database access with HTTP API calls,
 * enabling the Blazor WebAssembly client to communicate with the server.
 * 
 * Key Features:
 * ✅ JWT Authentication integration
 * ✅ Comprehensive error handling
 * ✅ JSON serialization with camelCase support
 * ✅ Pagination and search functionality
 * ✅ CRUD operations for obituary management
 * ✅ Debug logging for troubleshooting
 * 
 * Migration Benefits:
 * 🚀 Separation of concerns (UI ↔ API ↔ Database)
 * 🚀 Scalable architecture
 * 🚀 Cross-platform compatibility
 * 🚀 Testable service layer
 */

using MemorialRegistry.Shared.DTOs;
using MemorialRegistry.Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MemorialRegistry.BlazorWasm.Services;

public class ObituaryApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ObituaryApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// 📊 PAGINATION: Retrieves obituaries with server-side pagination
    /// Replaces: Direct EF queries with Skip/Take
    /// </summary>
    public async Task<PaginatedResponse<Obituary>> GetObituariesAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            Console.WriteLine($"📡 Loading obituaries page {page}...");
            Console.WriteLine($"🔗 API Base URL: {_httpClient.BaseAddress}");
            var response = await _httpClient.GetAsync($"/api/obituary/all?page={page}&pageSize={pageSize}");
            Console.WriteLine($"📋 Response Status: {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response JSON: {json}");

            // The API returns { data: [...], pagination: {...} }
            // We need to map this to our PaginatedResponse format
            var result = JsonSerializer.Deserialize<ApiResponse>(json, _jsonOptions);
            Console.WriteLine($"Deserialized result: Data count = {result?.Data?.Count}, Pagination = {result?.Pagination?.CurrentPage}/{result?.Pagination?.TotalPages}");

            var paginatedResponse = new PaginatedResponse<Obituary>
            {
                Data = result?.Data ?? new List<Obituary>(),
                Pagination = new PaginationInfo
                {
                    CurrentPage = result?.Pagination?.CurrentPage ?? 1,
                    PageSize = result?.Pagination?.PageSize ?? pageSize,
                    TotalCount = result?.Pagination?.TotalCount ?? 0,
                    TotalPages = result?.Pagination?.TotalPages ?? 1,
                    HasNext = result?.Pagination?.HasNextPage ?? false,
                    HasPrevious = result?.Pagination?.HasPreviousPage ?? false
                }
            };

            Console.WriteLine($"Returning {paginatedResponse.Data.Count} obituaries from API");
            return paginatedResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading obituaries: {ex.Message}");
            Console.WriteLine($"Exception type: {ex.GetType().Name}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return new PaginatedResponse<Obituary>();
        }
    }

    private class ApiResponse
    {
        public List<Obituary> Data { get; set; } = new();
        public ApiPagination? Pagination { get; set; }
    }

    private class ApiPagination
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public async Task<Obituary?> GetObituaryAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Obituary>($"/api/obituary/details/{id}", _jsonOptions);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            return null;
        }
    }

    public async Task<Obituary> CreateObituaryAsync(Obituary obituary)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/obituary", obituary, _jsonOptions);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Obituary>(json, _jsonOptions)!;
    }

    public async Task UpdateObituaryAsync(int id, Obituary obituary)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.PutAsJsonAsync($"/api/obituary/{id}", obituary, _jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteObituaryAsync(int id)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.DeleteAsync($"/api/obituary/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<Obituary>> SearchObituariesAsync(string name)
    {
        var response = await _httpClient.GetAsync($"/api/obituary/search?name={Uri.EscapeDataString(name)}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Obituary>>(json, _jsonOptions) ?? new List<Obituary>();
    }

    public async Task<GenerateBiographyResponse> GenerateBiographyAsync(GenerateBiographyRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/obituary/generate-biography", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GenerateBiographyResponse>(json, _jsonOptions)
                ?? new GenerateBiographyResponse { Success = false, ErrorMessage = "Failed to deserialize response" };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating biography: {ex.Message}");
            return new GenerateBiographyResponse
            {
                Success = false,
                ErrorMessage = $"Failed to generate biography: {ex.Message}"
            };
        }
    }
}