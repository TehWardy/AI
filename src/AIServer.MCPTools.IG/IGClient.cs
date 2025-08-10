using System.Text.Json;
using AIServer.MCPTools.IG.Models;

namespace AIServer.MCPTools.IG;

public class IGClient
{
    readonly HttpClient igClient;

    public IGClient(string hostUrl, string apiKey)
    {
        igClient = new()
        {
            BaseAddress = new Uri(hostUrl)
        };

        igClient.DefaultRequestHeaders
            .Add("X-IG-API-KEY", apiKey);
    }

    public async Task AuthenticateAsync(string userName, string password)
    {
        igClient.DefaultRequestHeaders.Remove("Version");
        igClient.DefaultRequestHeaders.Add("Version", "2");

        // Step 1: Authenticate
        var loginPayload = new
        {
            identifier = userName,
            password = password
        };

        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginPayload),
            System.Text.Encoding.UTF8,
            "application/json");

        var loginResponse = await igClient
            .PostAsync("session", loginContent);

        loginResponse.EnsureSuccessStatusCode();

        loginResponse.Headers.TryGetValues("X-SECURITY-TOKEN", out var securityTokenValues);
        loginResponse.Headers.TryGetValues("CST", out var cstValues);

        string securityToken = securityTokenValues.First();
        string cst = cstValues.First();

        igClient.DefaultRequestHeaders.Add("X-SECURITY-TOKEN", securityToken);
        igClient.DefaultRequestHeaders.Add("CST", cst);
    }

    public async Task<PriceData[]> GetPricingData(string epic, string resolution, int count)
    {
        igClient.DefaultRequestHeaders.Remove("Version");
        igClient.DefaultRequestHeaders.Add("Version", "2");

        string priceUrl = $"prices/{epic}/{resolution}/{count}";
        var priceResponse = await igClient.GetAsync(priceUrl);
        priceResponse.EnsureSuccessStatusCode();

        var priceJson = await priceResponse.Content.ReadAsStringAsync();

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        var prices = JsonSerializer.Deserialize<PriceResponse>(priceJson, options);

        return prices.Prices;
    }

    /// <summary>
    /// Fetches the market navigation hierarchy starting from a root node (or top-level if nodeId is null).
    /// </summary>
    public async Task<MarketNode> GetMarketHierarchyAsync(string nodeId = null)
    {
        igClient.DefaultRequestHeaders.Remove("Version");
        igClient.DefaultRequestHeaders.Add("Version", "1");

        string url = string.IsNullOrEmpty(nodeId) ? "marketnavigation" : $"marketnavigation/{nodeId}";
        var response = await igClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<MarketNode>(json, options);
    }

    /// <summary>
    /// Searches for markets by keyword (e.g., "AAPL" or "Tesla").
    /// </summary>
    public async Task<MarketDetail[]> SearchMarketsAsync(string searchTerm)
    {
        igClient.DefaultRequestHeaders.Remove("Version");
        igClient.DefaultRequestHeaders.Add("Version", "1");

        string url = $"markets?searchTerm={Uri.EscapeDataString(searchTerm)}";
        var response = await igClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<MarketSearchResponse>(json, options).Markets;
    }
}