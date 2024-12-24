// ============================================
// Arquivo: ApiConfig.cs
// ============================================

public class ApiConfig
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string CertPath { get; init; }
    public required string KeyPath { get; init; }
    public required string TokenUrl { get; init; }
    public required string ApiEndpoint { get; init; }
}

// ============================================
// Arquivo: IApiService.cs
// ============================================

public interface IApiService
{
    Task<string> GetTokenAsync();
    Task<string> FetchDataAsync();
}

// ============================================
// Arquivo: ApiService.cs
// ============================================

using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

public class ApiService(
    IHttpClientFactory httpClientFactory,
    ApiConfig config) : IApiService
{
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
    private readonly ApiConfig config = config;

    public async Task<string> GetTokenAsync()
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(new X509Certificate2(config.CertPath, config.KeyPath));

        var clientWithCert = httpClientFactory.CreateClient("WithCert");
        clientWithCert.DefaultRequestVersion = System.Net.HttpVersion.Version20;
        clientWithCert.DefaultRequestHeaders.Add("Accept", "application/json");

        var payload = new
        {
            client_id = config.ClientId,
            client_secret = config.ClientSecret
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await clientWithCert.PostAsync(config.TokenUrl, content);

        response.EnsureSuccessStatusCode(); // Lança a exceção original em caso de falha

        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return json.GetProperty("access_token").GetString()!;
    }

    public async Task<string> FetchDataAsync()
    {
        var token = await GetTokenAsync();
        var client = httpClientFactory.CreateClient("Default");
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(config.ApiEndpoint);

        response.EnsureSuccessStatusCode(); // Lança a exceção original em caso de falha

        return await response.Content.ReadAsStringAsync();
    }
}

// ============================================
// Arquivo: IProcessDataService.cs
// ============================================

public interface IProcessDataService
{
    Task ProcessDataAsync();
}

// ============================================
// Arquivo: ProcessDataService.cs
// ============================================

using System.Text.Json;

public class ProcessDataService(
    IApiService apiService) : IProcessDataService
{
    private readonly IApiService apiService = apiService;

    public async Task ProcessDataAsync()
    {
        try
        {
            var data = await apiService.FetchDataAsync();

            // Validação genética
            var isValid = ValidateData(data);
            if (!isValid)
            {
                Console.WriteLine("Dados inválidos após a validação.");
                return; // Retorna silenciosamente sem processar dados inválidos
            }

            // Passa os dados validados para a procedure
            await CallProcedureAsync(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no processamento dos dados: {ex.Message}");
        }
    }

    private bool ValidateData(string jsonData)
    {
        Console.WriteLine("Validando dados...");
        // Implementar lógica de validação genética
        return true; // Exemplo
    }

    private Task CallProcedureAsync(string jsonData)
    {
        Console.WriteLine("Chamando procedure com dados...");
        // Simula chamada da procedure
        return Task.CompletedTask;
    }
}// ============================================
// Arquivo: ApiConfig.cs
// ============================================

public class ApiConfig
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string CertPath { get; init; }
    public required string KeyPath { get; init; }
    public required string TokenUrl { get; init; }
    public required string ApiEndpoint { get; init; }
}

// ============================================
// Arquivo: IApiService.cs
// ============================================

public interface IApiService
{
    Task<string> GetTokenAsync();
    Task<string> FetchDataAsync();
}

// ============================================
// Arquivo: ApiService.cs
// ============================================

using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

public class ApiService(
    IHttpClientFactory httpClientFactory,
    ApiConfig config) : IApiService
{
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
    private readonly ApiConfig config = config;

    public async Task<string> GetTokenAsync()
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(new X509Certificate2(config.CertPath, config.KeyPath));

        var clientWithCert = httpClientFactory.CreateClient("WithCert");
        clientWithCert.DefaultRequestVersion = System.Net.HttpVersion.Version20;
        clientWithCert.DefaultRequestHeaders.Add("Accept", "application/json");

        var payload = new
        {
            client_id = config.ClientId,
            client_secret = config.ClientSecret
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await clientWithCert.PostAsync(config.TokenUrl, content);

        response.EnsureSuccessStatusCode(); // Lança a exceção original em caso de falha

        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return json.GetProperty("access_token").GetString()!;
    }

    public async Task<string> FetchDataAsync()
    {
        var token = await GetTokenAsync();
        var client = httpClientFactory.CreateClient("Default");
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(config.ApiEndpoint);

        response.EnsureSuccessStatusCode(); // Lança a exceção original em caso de falha

        return await response.Content.ReadAsStringAsync();
    }
}

// ============================================
// Arquivo: IProcessDataService.cs
// ============================================

public interface IProcessDataService
{
    Task ProcessDataAsync();
}

// ============================================
// Arquivo: ProcessDataService.cs
// ============================================

using System.Text.Json;

public class ProcessDataService(
    IApiService apiService) : IProcessDataService
{
    private readonly IApiService apiService = apiService;

    public async Task ProcessDataAsync()
    {
        try
        {
            var data = await apiService.FetchDataAsync();

            // Validação genética
            var isValid = ValidateData(data);
            if (!isValid)
            {
                Console.WriteLine("Dados inválidos após a validação.");
                return; // Retorna silenciosamente sem processar dados inválidos
            }

            // Passa os dados validados para a procedure
            await CallProcedureAsync(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no processamento dos dados: {ex.Message}");
        }
    }

    private bool ValidateData(string jsonData)
    {
        Console.WriteLine("Validando dados...");
        // Implementar lógica de validação genética
        return true; // Exemplo
    }

    private Task CallProcedureAsync(string jsonData)
    {
        Console.WriteLine("Chamando procedure com dados...");
        // Simula chamada da procedure
        return Task.CompletedTask;
    }
}
