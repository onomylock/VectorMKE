namespace VectorMKE.Models;

public class Mesh
{
    public int NodeXCount { get; set; }
    public int NodeYCount { get; set; }
    public List<Edge> Edges { get; set; }
    public List<Rectangle> Rectangles { get; set; }
}