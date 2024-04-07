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
    
    public static Mesh GenerateMeshByOptions(Vector2DOptions options)
    {
        var edgeCount = SetMeshExtension.GetGlobalElementCount(options.NodeXCount, options.NodeYCount);
        
        var mesh = new Mesh
        {
            Edges = new Edge[edgeCount].ToList(),
            Rectangles = [],
            NodeXCount = options.NodeXCount,
            NodeYCount = options.NodeYCount
        };

        var pointX = options.StartX;
        var currY = options.StartY;

        var dx = options.DischargeX;
        var dy = options.DischargeY;

        var stepX = Math.Abs(dx - 1) < 1e-5 ? (options.EndX - options.StartX) / options.NodeXCount
            : (options.EndX - options.StartX) * (dx - 1) / (Math.Pow(dx, options.NodeXCount - 1) - 1);
        
        var stepY = Math.Abs(dy - 1) < 1e-5 ? (options.EndY - options.StartY) / options.NodeXCount 
            : (options.EndY - options.StartY) * (dy - 1) / (Math.Pow(dy, options.NodeYCount - 1) - 1);
        
        for (int j = 0; j < options.NodeYCount; j++, stepY *= dy, currY += stepY)
        {
            var stepXTmp = stepX;
            var currX = pointX;
            
            for (int i = 0; i < options.NodeXCount; i++, stepXTmp *= dx, currX += stepXTmp)    
            {
                // mesh.Edges.Add(new Edge
                // {
                //     P1 = new Point2D
                //     {
                //         X = currX,
                //         Y = currY
                //     },
                //     P2 = new Point2D
                //     {   
                //         X = currX + stepXTmp,
                //         Y = currY + stepY
                //     }
                // });

                var elems = GetElements(i, j, options.NodeXCount, options.NodeYCount);

                //mesh.Edges[elems[0]] = mesh.Edges[elems[0]] == null ? new Edge();
                mesh.Edges[elems[0]] ??= new Edge()
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
                    }
                };
                mesh.Edges[elems[1]] ??= new Edge()
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
                    }
                };
                
                mesh.Edges[elems[2]] ??= new Edge()
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
                    }
                };
                
                mesh.Edges[elems[3]] ??= new Edge()
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
                    }
                };
                
                // foreach (var elem in elems)
                // {
                //     if (mesh.Edges[elem] == null)
                //         mesh.Edges[elem] = new Edge()
                //         {
                //             P1 = new Point2D()
                //             {
                //                 X = currX,
                //                 
                //             }
                //         };
                // }
                
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
    
    
}