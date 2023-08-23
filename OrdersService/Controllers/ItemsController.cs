using Microsoft.AspNetCore.Mvc;
using OrdersService.Services;

namespace OrdersService.Controllers;

[ApiController]
[Route("[action]")]
public class ItemsController : ControllerBase
{
    private readonly ILogger<ItemsController> logger;
    private readonly IItemsStateService itemsStateService;

    public ItemsController(ILogger<ItemsController> logger, IItemsStateService itemsStateService)
    {
        this.logger = logger;
        this.itemsStateService = itemsStateService;
    }

    [HttpPost]
    public async Task<string> CreateItems(int count) => await itemsStateService.CreateItems(count);
    [HttpGet]
    public async Task<string> GetItem(Guid id) => await itemsStateService.GetItem(id);
    [HttpDelete]
    public async Task<string> DeleteItem(Guid id) => await itemsStateService.DeleteItem(id);
}