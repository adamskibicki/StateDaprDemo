using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers;

[ApiController]
[Route("[action]")]
public class ItemsManagementController : ControllerBase
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string baseURL;
    private readonly string STATE_SERVICE_NAME;
    private readonly ILogger<ItemsManagementController> logger;

    public ItemsManagementController(IHttpClientFactory httpClientFactory, ILogger<ItemsManagementController> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
        baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" 
                                                                                         + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");
        STATE_SERVICE_NAME = Environment.GetEnvironmentVariable("STATE_SERVICE_NAME") ?? "svcsvc-app";
    }
    
    [HttpGet]
    public async Task<string> GetItems(Guid id)
    {
        var httpClient = CreateHttpClient();

        var url = $"{baseURL}/GetItem?id={id}";
        
        logger.LogInformation(url);
        
        return await httpClient.GetStringAsync(url);
    }
    
    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("dapr-app-id", STATE_SERVICE_NAME);
        return httpClient;
    }
}