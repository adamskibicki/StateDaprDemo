using Microsoft.AspNetCore.Mvc;
using StateDaprDemo.Services;

namespace StateDaprDemo.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
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
    [HttpGet]
    public async Task<string> GetItems() => await itemsStateService.GetItems();
    [HttpDelete]
    public async Task<string> DeleteItem(Guid id) => await itemsStateService.DeleteItem(id);
}