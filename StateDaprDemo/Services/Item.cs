using System.Text.Json.Serialization;

namespace StateDaprDemo.Services;

public record Item([property: JsonPropertyName("id")] Guid id,
    [property: JsonPropertyName("value")] string value);