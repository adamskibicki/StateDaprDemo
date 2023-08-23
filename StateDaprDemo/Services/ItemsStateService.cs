using System.Text;
using System.Text.Json;

namespace StateDaprDemo.Services;

public class ItemsStateService : IItemsStateService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string baseURL;
    private const string DAPR_STATE_STORE = "items";

    public ItemsStateService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
        baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" 
            + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");
    }

    public async Task<string> CreateItems(int count)
    {
        var httpClient = CreateHttpClient();

        var items = Enumerable.Range(0, count)
            .Select(i => new Item(Guid.NewGuid(), $"Value is: {i}"))
            .ToArray();

        var itemsJson = JsonSerializer.Serialize(
            items.Select(i => new
            {
                key = i.id.ToString(),
                value = i
            }).ToArray()
        );

        var state = new StringContent(itemsJson, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{baseURL}/v1.0/state/{DAPR_STATE_STORE}", state);
        return response.ToString();
    }

    public async Task<string> GetItem(Guid id)
    {
        var httpClient = CreateHttpClient();

        return await httpClient.GetStringAsync($"{baseURL}/v1.0/state/{DAPR_STATE_STORE}/{id.ToString()}");
    }

    public async Task<string> GetItems()
    {
        var httpClient = CreateHttpClient();

        return await httpClient.GetStringAsync($"{baseURL}/v1.0/state/{DAPR_STATE_STORE}");
    }

    public async Task<string> DeleteItem(Guid id)
    {
        var httpClient = CreateHttpClient();

        var response = await httpClient.DeleteAsync($"{baseURL}/v1.0/state/{DAPR_STATE_STORE}/{id.ToString()}");
        return response.ToString();
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}