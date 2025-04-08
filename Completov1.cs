// Infrastructure/Http/TokenInterceptor.cs
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YourProject.Infrastructure.Http;

public interface ITokenService
{
    Task<string> GetTokenAsync();
}

public class TokenInterceptor(ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenService.GetTokenAsync();
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}

// Infrastructure/Http/HttpClientSetup.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Polly;
using Polly.Extensions.Http;

namespace YourProject.Infrastructure.Http;

public static class HttpClientSetup
{
    public static IServiceCollection AddHttpClients(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Registra o interceptador
        services.AddTransient<TokenInterceptor>();

        // Configuração do HttpClient para o AtlasApi
        services.AddHttpClient("AtlasApi", client =>
        {
            // Configuração base do cliente HTTP
            client.BaseAddress = new Uri(configuration["AtlasApi:BaseUrl"]);
            client.Timeout = TimeSpan.FromSeconds(30);
            
            // Headers padrão
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "AtlasApiClient");
        })
        // Adiciona o interceptador de token
        .AddHttpMessageHandler<TokenInterceptor>()
        // Adiciona política de retry
        .AddPolicyHandler(GetRetryPolicy())
        // Adiciona circuit breaker
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    // Política de retry para tentativas em caso de falha
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    // Política de circuit breaker para evitar sobrecarga
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}

// Services/Interfaces/IAtlasService.cs
namespace YourProject.Services.Interfaces;

public interface IAtlasService
{
    Task<TResponse> GetAsync<TResponse>(string endpoint);
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request);
    // Adicione outros métodos conforme necessário
}

// Services/AtlasService.cs
using System.Net.Http;
using System.Net.Http.Json;
using YourProject.Services.Interfaces;

namespace YourProject.Services;

public class AtlasService(IHttpClientFactory httpClientFactory) : IAtlasService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AtlasApi");

    public async Task<TResponse> GetAsync<TResponse>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>() 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>()
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}

// Controllers/ExemploController.cs
using Microsoft.AspNetCore.Mvc;
using YourProject.Services.Interfaces;

namespace YourProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExemploController(IAtlasService atlasService) : ControllerBase
{
    [HttpGet("dados")]
    public async Task<ActionResult<SeuTipoDeResposta>> ObterDados()
    {
        try
        {
            var resultado = await atlasService.GetAsync<SeuTipoDeResposta>("endpoint/dados");
            return Ok(resultado);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, "Erro ao comunicar com a API Atlas");
        }
    }

    [HttpPost("dados")]
    public async Task<ActionResult<SeuTipoDeResposta>> EnviarDados([FromBody] SeuTipoDeRequest request)
    {
        try
        {
            var resultado = await atlasService.PostAsync<SeuTipoDeRequest, SeuTipoDeResposta>(
                "endpoint/dados", 
                request
            );
            return Ok(resultado);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, "Erro ao comunicar com a API Atlas");
        }
    }
}

// Program.cs
using YourProject.Infrastructure.Http;
using YourProject.Services;
using YourProject.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, SeuServicoDeToken>();
builder.Services.AddScoped<IAtlasService, AtlasService>();
builder.Services.AddHttpClients(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// appsettings.json
{
    "AtlasApi": {
        "BaseUrl": "https://api.atlas.exemplo.com/"
    }
}
