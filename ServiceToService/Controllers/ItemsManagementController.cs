using Microsoft.AspNetCore.Mvc;

namespace ServiceToService.Controllers;

[ApiController]
[Route("[action]")]
public class ItemsManagementController : ControllerBase
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string baseURL;
    private readonly string STATE_SERVICE_NAME;

    public ItemsManagementController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
        baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" 
            + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");
        STATE_SERVICE_NAME = Environment.GetEnvironmentVariable("STATE_SERVICE_NAME") ?? "svcsvc-app";
    }
    
    [HttpGet]
    public async Task<string> GetItems()
    {
        var httpClient = CreateHttpClient();

        return await httpClient.GetStringAsync($"{baseURL}/v1.0/invoke/{STATE_SERVICE_NAME}/GetItems");
    }
    
    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}