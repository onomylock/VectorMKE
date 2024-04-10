using System.Text.Json.Serialization;
using VectorMKE.Converters;
using VectorMKE.Enums;

namespace VectorMKE.Options;

public record InputOptions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InputType InputType { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DimensionalType DimensionalType { get; set; }
    
    [JsonConverter(typeof(DictionaryTKeyEnumTValueConverter))]
    public Dictionary<FunctionType, string> Functions { get; set; }
    
    public List<BoundOption> BoundOptions { get; set; }
    
    public Vector2DOptions Vector2DOptions { get; set; }
    
    public Coord2DOptions Coord2DOptions { get; set; }
    
    public List<double> TrueVector { get; set; } 
    
    public int NodeXCount { get; set; }
    public int NodeYCount { get; set; }
}