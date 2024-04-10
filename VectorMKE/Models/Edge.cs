using System.Text.Json.Serialization;
using VectorMKE.Enums;

namespace VectorMKE.Models;

public class Edge
{
    public Point2D P1 { get; set; }
    public Point2D P2 { get; set; }
    
    public double Hx { get; set; }
    public double Hy { get; set; }
    
    public Point2D Normal { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BoundType BoundType { get; set; }
}