using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using VectorMKE.Extensions;
using VectorMKE.Models;

namespace VectorMKE.Services;

public class GlobalMatrixGenService
{
    public Global GetGlobalData(Mesh mesh)
    {
        var data = new Global();
        var elemCount = SetMeshExtension.GetGlobalElementCount(mesh.NodeXCount, mesh.NodeYCount);
        var indexes = GetCoordinateMatrixData(mesh);
        data.A = CreateMatrix.SparseFromCoordinateFormat(elemCount, elemCount, indexes.columnIndexes.Length,
            indexes.rowIndexes, indexes.columnIndexes, new double[indexes.columnIndexes.Length]);

        data.F = DenseVector.Create(elemCount, 0);
        return data;
    }

    private (int[] rowIndexes, int[] columnIndexes) GetCoordinateMatrixData(Mesh mesh)
    {
        var rowIndexes = new List<int>();
        var columnIndexes = new List<int>();
        
        foreach (var rect in mesh.Rectangles)
        {
            foreach (var ielem in rect.Elements)
            {
                foreach (var jelem in rect.Elements)
                {
                    rowIndexes.Add(ielem);
                    columnIndexes.Add(jelem);
                }
            }
        }
        
        return (rowIndexes.ToArray(), columnIndexes.ToArray());
    }

}