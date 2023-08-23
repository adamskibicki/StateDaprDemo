using Microsoft.AspNetCore.Mvc;
using Shared;

namespace InventoryService.Controllers;

[ApiController]
[Route("[action]")]
public class InventoryController : ControllerBase
{
    [HttpGet]
    public IActionResult List()
    {
        return Ok(new []
        {
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
            new InventoryItem(Guid.NewGuid(),$"Some name with random guid: {Guid.NewGuid()}"),
        });
    }
}