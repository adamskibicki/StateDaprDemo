using System.Text.Json.Serialization;

namespace OrdersService.Services;

public record Item([property: JsonPropertyName("id")] Guid id,
    [property: JsonPropertyName("value")] string value);