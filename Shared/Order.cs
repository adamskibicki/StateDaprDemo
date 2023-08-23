using System.Text.Json.Serialization;

namespace Shared;

public record Order([property: JsonPropertyName("id")] Guid id,
    [property: JsonPropertyName("value")] string value);
    
public record InventoryItem([property: JsonPropertyName("id")] Guid id,
    [property: JsonPropertyName("name")] string name);