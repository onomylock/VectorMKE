using VectorMKE.Enums;
using VectorMKE.Models;
using VectorMKE.Options;
using Math = System.Math;

namespace VectorMKE.Extensions;

public static class SetMeshExtension
{
    private static int[] GetElements(int i, int j, int nodeX, int nodeY)
    {
        return [nodeX * (j + 1) + nodeY * j + i + j,
            nodeX * (j + 1) + nodeY * j + i + j + 1,
            i + nodeX * 2 * j + j, 
            i + nodeX * 2 * (j + 1) + j + 1
        ];
    }
    
    

    public static int GetGlobalElementCount(int nodeX, int nodeY)
    {
        return nodeX + nodeX * 2 * nodeY + nodeY;
    }
    
    public static Mesh GenerateMeshByOptionsVector2D(InputOptions options)
    {
        var edgeCount = GetGlobalElementCount(options.NodeXCount, options.NodeYCount);
        
        var mesh = new Mesh
        {
            Edges = new Edge[edgeCount].ToList(),
            Rectangles = [],
            NodeXCount = options.NodeXCount,
            NodeYCount = options.NodeYCount
        };

        var pointX = options.Vector2DOptions.StartX;
        var currY = options.Vector2DOptions.StartY;

        var dx = options.Vector2DOptions.DischargeX;
        var dy = options.Vector2DOptions.DischargeY;

        var stepX = Math.Abs(dx - 1) < 1e-5
            ? (options.Vector2DOptions.EndX - options.Vector2DOptions.StartX) / options.NodeXCount
            : (options.Vector2DOptions.EndX - options.Vector2DOptions.StartX) * (dx - 1) /
              (Math.Pow(dx, options.NodeXCount - 1) - 1);
        
        var stepY = Math.Abs(dy - 1) < 1e-5
            ? (options.Vector2DOptions.EndY - options.Vector2DOptions.StartY) / options.NodeXCount 
            : (options.Vector2DOptions.EndY - options.Vector2DOptions.StartY) * (dy - 1) /
              (Math.Pow(dy, options.NodeYCount - 1) - 1);
        
        for (var j = 0; j < options.NodeYCount; j++, stepY *= dy, currY += stepY)
        {
            var stepXTmp = stepX;
            var currX = pointX;
            
            for (var i = 0; i < options.NodeXCount; i++, stepXTmp *= dx, currX += stepXTmp)    
            {
                var elems = GetElements(i, j, options.NodeXCount, options.NodeYCount);
                mesh.Edges[elems[0]] = new Edge()
                {
                    P1 = new Point2D()
                    {
                        X = currX,
                        Y = currY
                    },
                    P2 = new Point2D()
                    {
                        X = currX,
                        Y = currY + stepY
                    },
                    Normal = new Point2D
                    {
                       X = 0,
                       Y = -1
                    },
                    BoundType = Math.Abs(options.Vector2DOptions.StartX - currX) < 1e-5
                        ? options.BoundOptions.First(x => x.SideType == BoundingBoxSideType.Left).BoundType
                        : BoundType.None 
                };
                mesh.Edges[elems[1]] = new Edge()
                {
                    P1 = new Point2D()
                    {
                        X = currX + stepXTmp,
                        Y = currY
                    },
                    P2 = new Point2D()
                    {
                        X = currX + stepXTmp,
                        Y = currY + stepY
                    },
                    Normal = new Point2D
                    {
                        X = 0,
                        Y = 1
                    },
                    BoundType = Math.Abs(options.Vector2DOptions.EndX - (currX + stepXTmp)) < 1e-5
                        ? options.BoundOptions.First(x => x.SideType == BoundingBoxSideType.Right).BoundType
                        : BoundType.None
                };
                
                mesh.Edges[elems[2]] = new Edge()
                {
                    P1 = new Point2D()
                    {
                        X = currX,
                        Y = currY
                    },
                    P2 = new Point2D()
                    {
                        X = currX + stepXTmp,
                        Y = currY
                    },
                    Normal = new Point2D
                    {
                        X = -1,
                        Y = 0
                    },
                    BoundType = Math.Abs(options.Vector2DOptions.StartY - currY) < 1e-5
                        ? options.BoundOptions.First(x => x.SideType == BoundingBoxSideType.Bottom).BoundType
                        : BoundType.None
                };
                
                mesh.Edges[elems[3]] = new Edge()
                {
                    P1 = new Point2D()
                    {
                        X = currX,
                        Y = currY + stepY
                    },
                    P2 = new Point2D()
                    {
                        X = currX + stepXTmp,
                        Y = currY + stepY
                    },
                    Normal = new Point2D
                    {
                        X = 1,
                        Y = 0
                    },
                    BoundType = Math.Abs(options.Vector2DOptions.EndY - (currY + stepY)) < 1e-5
                        ? options.BoundOptions.First(x => x.SideType == BoundingBoxSideType.Top).BoundType
                        : BoundType.None
                };
                
                mesh.Rectangles.Add(new Rectangle()
                {
                    Elements = elems,
                    Hx = stepXTmp,
                    Hy = stepY
                });
            }
        }
        
        return mesh;
    }

    public static Mesh GenerateMeshByCoord2DOptions(InputOptions options)
    {
        var mesh = new Mesh
        {
            Edges = options.Coord2DOptions.Edges,
            Rectangles = [],
            NodeXCount = options.NodeXCount,
            NodeYCount = options.NodeYCount
        };

        for (var j = 0; j < options.NodeYCount; j++)
        {
            for (var i = 0; i < options.NodeXCount; i++)
            {
                var elems = GetElements(i, j, options.NodeXCount, options.NodeYCount);

                var hx = mesh.Edges[elems[3]].P2.X - mesh.Edges[elems[0]].P1.X;
                var hy = mesh.Edges[elems[3]].P2.Y - mesh.Edges[elems[0]].P1.Y;
                
                mesh.Rectangles.Add(new Rectangle()
                {
                    Elements = elems,
                    Hx = hx,
                    Hy = hy
                });
            }
        }
        
        return mesh;
    }
}