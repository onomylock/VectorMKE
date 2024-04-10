using System.Text.Json;
using MathNet.Numerics.LinearAlgebra.Double;
using VectorMKE.Enums;
using VectorMKE.Extensions;
using VectorMKE.Models;
using VectorMKE.Options;

namespace VectorMKE.Services;

public class SolverService
{
    private readonly IFunctionService _functionService;
    private readonly ICalculationService _calculationService;
    private readonly InputOptions _options;
    private Mesh _mesh;
    private Global _global;

    public SolverService(string jsonPath)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), jsonPath);

        using var sr = new StreamReader(path);
        var json = sr.ReadToEnd();

        _options = JsonSerializer.Deserialize<InputOptions>(json) ?? throw new InvalidOperationException();
        _functionService = new FunctionService(_options.Functions);
        _mesh = _options.InputType switch
        {
            InputType.Vector2D => SetMeshExtension.GenerateMeshByOptionsVector2D(_options),
            InputType.Coord2D => SetMeshExtension.GenerateMeshByCoord2DOptions(_options),
            _ => throw new ArgumentOutOfRangeException()
        };
        var genGlobal = new GlobalMatrixGenService();
        _global = genGlobal.GetGlobalData(_mesh);
        _calculationService = new CalculationService(_functionService, _mesh);
        Solve();
    }

    private void GenGlobalMatrix()
    {
        foreach (var rectangle in _mesh.Rectangles)
        {
            var localG = _calculationService.CalculateLocalG(rectangle);
            var localM = _calculationService.CalculateLocalM(rectangle);
            var localF = _calculationService.CalculateLocalF(rectangle);
            var localB = _calculationService.CalculateLocalB(localM, localF);

            for (var i = 0; i < rectangle.Elements.Length; i++)
            {
                for (var j = 0; j < rectangle.Elements.Length; j++)
                {
                    _global.A[rectangle.Elements[i], rectangle.Elements[j]] += localG[i, j] + localM[i, j];

                }

                _global.F[rectangle.Elements[i]] += localB[i];
            }
        }
    }

    private void SetMainBoundaryConditions()
    {
        (double ty, double tx) tetta;
        foreach (var edge in _mesh.Edges.Where(x => x.BoundType == BoundType.First))
        {
            var fx = _functionService.GetFuncValue(FunctionType.FunX, Math.Abs(edge.P1.X + edge.P2.X) / 2);
            var fy = _functionService.GetFuncValue(FunctionType.FunY, y: Math.Abs(edge.P1.Y + edge.P2.Y) / 2);
            tetta.ty = -edge.Normal.Y;
            tetta.tx = edge.Normal.X;

            var q = fx * tetta.tx + fy * tetta.ty;
            var index = _mesh.Edges.IndexOf(edge);
            _global.F[index] = q * 1e+20;
            _global.A[index, index] = 1e+20;
        }
    }
    
    private void SetNaturalBoundaryCondition()
    {
        foreach (var edge in _mesh.Edges.Where(x => x.BoundType == BoundType.Second))
        {
            int i = _mesh.Edges.IndexOf(edge);
            var tetta = _functionService.GetFuncValue(FunctionType.Tetta, (edge.P2.X + edge.P1.X) / 2,
                (edge.P2.Y + edge.P1.Y) / 2);
            var gamma = _functionService.GetFuncValue(FunctionType.Gamma);
            var b = tetta * (-edge.Normal.Y * (edge.P2.X - edge.P1.X) + edge.Normal.X * (edge.P2.Y - edge.P1.Y)) /
                    gamma;
            _global.F[i] += b;
        }
    }

    public void Solve()
    {
        GenGlobalMatrix();
        SetNaturalBoundaryCondition();
        SetMainBoundaryConditions();

        var res = _global.A.Solve(_global.F);
        res.ToList().ForEach(_ => Console.WriteLine(_.ToString()));
        var trueVec = DenseVector.OfArray(_options.TrueVector.ToArray());
        Console.WriteLine((res - trueVec).InfinityNorm() / trueVec.InfinityNorm());
    }
}