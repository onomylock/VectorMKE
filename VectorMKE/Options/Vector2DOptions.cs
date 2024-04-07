using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using VectorMKE.Converters;
using VectorMKE.Enums;

namespace VectorMKE.Options;

[DataContract]
public record Vector2DOptions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DimensionalType DimensionalType { get; set; }
    
    public int NodeXCount { get; set; }
    public int NodeYCount { get; set; }
    
    public double DischargeX { get; set; }
    public double DischargeY { get; set; }
    
    public double StartX { get; set; }
    public double StartY { get; set; }
    
    public double EndX { get; set; }
    public double EndY { get; set; }
    
    [JsonConverter(typeof(DictionaryTKeyEnumTValueConverter))]
    public Dictionary<FunctionType, string> Functions { get; set; }
}