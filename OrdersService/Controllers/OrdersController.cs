using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace OrdersService.Controllers;

[ApiController]
[Route("[action]")]
public class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string baseURL;
    private const string DAPR_STATE_STORE = "orders";

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
        baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":"
            + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Order order)
    {
        var httpClient = CreateHttpClient();

        var ordersJson = JsonSerializer.Serialize(
            new[]
            {
                new
                {
                    key = order.id.ToString(),
                    value = order
                }
            }
        );

        var state = new StringContent(ordersJson, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{baseURL}/v1.0/state/{DAPR_STATE_STORE}", state);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Fetch(Guid id)
    {
        var httpClient = CreateHttpClient();

        var order = await httpClient.GetFromJsonAsync<Order>(
            $"{baseURL}/v1.0/state/{DAPR_STATE_STORE}/{id.ToString()}");

        if (order != null)
            return Ok(order);

        return BadRequest();
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}