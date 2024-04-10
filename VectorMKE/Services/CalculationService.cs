using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using VectorMKE.Enums;
using VectorMKE.Models;

namespace VectorMKE.Services;

public interface ICalculationService
{
    Matrix<double> CalculateLocalG(Rectangle rect);
    Matrix<double> CalculateLocalM(Rectangle rect);
    Vector<double> CalculateLocalF(Rectangle rect);
    Vector<double> CalculateLocalB(Matrix<double> M, Vector<double> f);
}

public class CalculationService(IFunctionService functionService, Mesh mesh) : ICalculationService
{
    private Matrix<double> Mtmp = DenseMatrix.OfArray(new double[,]
        {
            { 2, 1, 0, 0 },
            { 1, 2, 0, 0 },
            { 0, 0, 2, 1 },
            { 0, 0, 1, 2 }
        });
    
    public Matrix<double> CalculateLocalG(Rectangle rect)
    {
        double mu = functionService.GetFuncValue(FunctionType.Mu);

        var hyhx = rect.Hy / rect.Hx;
        var hxhy = rect.Hx / rect.Hy;
        
        return DenseMatrix.OfArray(new double[,]
        {
            {hyhx / mu, -hyhx / mu, -1 / mu, 1 / mu },
            {-hyhx / mu, hyhx / mu, 1 / mu, -1 / mu },
            {-1 / mu, 1 / mu, hxhy / mu, -hxhy / mu },
            {1 / mu, -1 / mu, -hxhy / mu, hxhy / mu }
        });
    }

    public Matrix<double> CalculateLocalM(Rectangle rect)
    {
        var coeff = functionService.GetFuncValue(FunctionType.Gamma)
            * rect.Hx * rect.Hy / 6;
        return Mtmp * coeff;
    }

    public Vector<double> CalculateLocalF(Rectangle rect)
    {
        var vectF = DenseVector.Create(4, 0);
        
        vectF[0] = functionService.GetFuncValue(FunctionType.FunX, mesh.Edges[rect.Elements[0]].P1.X,
            Math.Abs((mesh.Edges[rect.Elements[0]].P2.Y - mesh.Edges[rect.Elements[0]].P1.Y) / 2));
        
        vectF[1] = functionService.GetFuncValue(FunctionType.FunX, mesh.Edges[rect.Elements[1]].P1.X,
            Math.Abs((mesh.Edges[rect.Elements[1]].P2.Y - mesh.Edges[rect.Elements[1]].P1.Y) / 2));
        
        vectF[2] = functionService.GetFuncValue(FunctionType.FunY, Math.Abs(mesh.Edges[rect.Elements[2]].P2.X - mesh.Edges[rect.Elements[2]].P1.X),
            mesh.Edges[rect.Elements[2]].P1.Y);
        
        vectF[3] = functionService.GetFuncValue(FunctionType.FunY, Math.Abs(mesh.Edges[rect.Elements[3]].P2.X - mesh.Edges[rect.Elements[3]].P1.X),
            mesh.Edges[rect.Elements[3]].P1.Y);
        // vectF[0] = functionService.GetFuncValue(FunctionType.FunX, Math.Abs(mesh.Edges[rect.Elements[2]].P2.X - mesh.Edges[rect.Elements[2]].P1.X) / 2,
        //     mesh.Edges[rect.Elements[2]].P1.Y);
        //
        // vectF[1] = functionService.GetFuncValue(FunctionType.FunX, Math.Abs(mesh.Edges[rect.Elements[3]].P2.X - mesh.Edges[rect.Elements[3]].P1.X) / 2,
        //     mesh.Edges[rect.Elements[3]].P1.Y);
        //
        // vectF[2] = functionService.GetFuncValue(FunctionType.FunY, mesh.Edges[rect.Elements[0]].P1.X,
        //     Math.Abs((mesh.Edges[rect.Elements[0]].P2.Y - mesh.Edges[rect.Elements[0]].P1.Y) / 2));
        //
        // vectF[3] = functionService.GetFuncValue(FunctionType.FunY, mesh.Edges[rect.Elements[1]].P1.X,
        //     Math.Abs((mesh.Edges[rect.Elements[1]].P2.Y - mesh.Edges[rect.Elements[1]].P1.Y) / 2));
        return vectF;
    }

    public Vector<double> CalculateLocalB(Matrix<double> m, Vector<double> f)
    {
        return m.Multiply(f);
    }
}