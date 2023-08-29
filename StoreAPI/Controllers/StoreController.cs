using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace StoreAPI.Controllers;

[ApiController]
[Route("[action]")]
public class StoreController : ControllerBase
{
    private readonly IHttpClientFactory httpClientFactory;

    private readonly string baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":"
        + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");

    private readonly string ORDERS_SERVICE_NAME =
        Environment.GetEnvironmentVariable("ORDERS_SERVICE_NAME") ?? "orders-app";
    
    private readonly string INVENTORY_SERVICE_NAME =
        Environment.GetEnvironmentVariable("INVENTORY_SERVICE_NAME") ?? "inventory-app";

    private readonly ILogger<StoreController> logger;
    
    public StoreController(IHttpClientFactory httpClientFactory, ILogger<StoreController> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody]Order order)
    {
        var httpClient = CreateHttpClient(ORDERS_SERVICE_NAME);

        var url = $"{baseURL}/Create";

        await httpClient.PostAsJsonAsync(url, order);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> FetchOrder([FromQuery]Guid id)
    {
        var httpClient = CreateHttpClient(ORDERS_SERVICE_NAME);

        var url = $"{baseURL}/Fetch?id={id}";

        var response = await httpClient.GetAsync(url);
        
        var jsonString = await response.Content.ReadAsStringAsync();
        
        return Ok(JsonSerializer.Deserialize<Order>(jsonString));
    }

    [HttpGet]
    public async Task<IActionResult> ListInventory()
    {
        var httpClient = CreateHttpClient(INVENTORY_SERVICE_NAME);

        var url = $"{baseURL}/List";

        var response = await httpClient.GetAsync(url);
        
        var jsonString = await response.Content.ReadAsStringAsync();

        logger.LogWarning("test if logged correctly - start");
        logger.LogWarning(jsonString);
        logger.LogWarning("test if logged correctly - end");
        
        return Ok(JsonSerializer.Deserialize<InventoryItem[]>(jsonString));
    }

    private HttpClient CreateHttpClient(string serviceName)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("dapr-app-id", serviceName);
        return httpClient;
    }
}