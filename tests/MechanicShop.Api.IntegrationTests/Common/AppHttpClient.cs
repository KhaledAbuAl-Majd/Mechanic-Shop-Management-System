using System.Net.Http.Headers;
using System.Net.Http.Json;
using MechanicShop.Api.IntegrationTests.Common.EndpointsPath;
using MechanicShop.Api.Requests.V1.Identity;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Infrastructure.Identity;
using MechanicShop.Tests.Common.Security;

namespace MechanicShop.Api.IntegrationTests.Common;

public class AppHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public AppHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TokenDto> GenerateTokenAsync(string email, string password, CancellationToken ct = default)
    {
        var request = new GenerateTokenRequest(email, password);

        var response = await _httpClient.PostAsJsonAsync(IdentityEndpointsPath.GenerateToken, request, ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Token generation failed with status code {response.StatusCode}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenDto>();

        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Token response is null.");
        }

        return tokenResponse;
    }

    public async Task<TokenDto> GenerateTokenAsync(AppUser user, CancellationToken ct = default)
    {
        return await GenerateTokenAsync(user.Email!, user.Email!, ct);
    }

    public async Task<TokenDto> GenerateManagerTokenAsync(CancellationToken ct = default)
    {
        return await GenerateTokenAsync(TestUsers.Manager, ct);
    }

    public async Task<TokenDto> GenerateLaborTokenAsync(CancellationToken ct = default)
    {
        return await GenerateTokenAsync(TestUsers.Labor01, ct);
    }

    public void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void SetAuthorizationHeader(TokenDto token)
    {
        SetAuthorizationHeader(token.AccessToken);
    }

    public void ClearAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken ct = default)
    {
        return await _httpClient.GetAsync(requestUri, ct);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default)
    {
        return await _httpClient.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value, CancellationToken ct = default)
    {
        return await _httpClient.PostAsJsonAsync<T>(requestUri, value, ct);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value, CancellationToken ct = default)
    {
        return await _httpClient.PutAsJsonAsync<T>(requestUri, value, ct);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken ct = default)
    {
        return await _httpClient.DeleteAsync(requestUri, ct);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
