using System.Text.Json.Serialization;

namespace TatsConverter;

public class TattooData
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("section")]
    public string Section { get; set; }

    [JsonPropertyName("texture")]
    public string Texture { get; set; }

    [JsonPropertyName("area")]
    public string Area { get; set; }
}
