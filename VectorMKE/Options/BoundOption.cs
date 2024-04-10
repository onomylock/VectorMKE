using System.Collections.Specialized;
using System.Text.Json.Serialization;
using VectorMKE.Enums;

namespace VectorMKE.Options;

public record BoundOption
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoundingBoxSideType SideType { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoundType BoundType { get; set; }
    public string Function { get; set; }
}