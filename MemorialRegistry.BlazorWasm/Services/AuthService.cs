using MemorialRegistry.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;

namespace MemorialRegistry.BlazorWasm.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerOptions _jsonOptions;
    public bool IsInitialized { get; private set; }

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request, _jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(json, _jsonOptions);
                
                if (authResponse != null)
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "token", authResponse.Token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "email", authResponse.Email);
                    
                    // Set authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);
                }
                
                return authResponse;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "token");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "email");
        
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "token");
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetEmailAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "email");
        }
        catch
        {
            return null;
        }
    }

    public async Task InitializeAsync()
    {
        if (!IsInitialized)
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            IsInitialized = true;
        }
    }
}

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    public CustomAuthenticationStateProvider(AuthService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Initialize AuthService if not already done
            if (!_authService.IsInitialized)
            {
                await _authService.InitializeAsync();
            }
            
            var token = await _authService.GetTokenAsync();
            var email = await _authService.GetEmailAsync();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Simple token validation - in production, you'd validate the JWT properly
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, email) // Simplified - would extract from JWT
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting authentication state: {ex.Message}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task NotifyAuthenticationStateChangedAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }
}