using System.Text.Json.Serialization;

namespace VectorMKE.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DimensionalType
{
    Two,
    Three
}