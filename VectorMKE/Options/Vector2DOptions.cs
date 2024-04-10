using System.Runtime.Serialization;

namespace VectorMKE.Options;

[DataContract]
public record Vector2DOptions
{
    public double DischargeX { get; set; }
    public double DischargeY { get; set; }
    
    public double StartX { get; set; }
    public double StartY { get; set; }
    
    public double EndX { get; set; }
    public double EndY { get; set; }
    
    
    
    
}