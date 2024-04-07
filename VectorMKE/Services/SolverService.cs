using System.Text.Json;
using MathNet.Numerics.LinearAlgebra;
using VectorMKE.Extensions;
using VectorMKE.Models;
using VectorMKE.Options;

namespace VectorMKE.Services;

public class SolverService
{
    private readonly IExpressionEvaluatorService _expressionEvaluatorService;
    private readonly ICalculationService _calculationService;
    private Mesh _mesh;
    private Global _global;
    
    public SolverService(string jsonPath)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), jsonPath);
        
        using var sr = new StreamReader(path);
        var json = sr.ReadToEnd();
        
        var options = JsonSerializer.Deserialize<Vector2DOptions>(json) ?? throw new InvalidOperationException();
        _expressionEvaluatorService = new ExpressionEvaluatorService(options.Functions);
        _mesh = SetMeshExtension.GenerateMeshByOptions(options);
        var genGlobal = new GlobalMatrixGenService();
        _global = genGlobal.GetGlobalData(_mesh);
        _calculationService = new CalculationService(_expressionEvaluatorService, _mesh);
        Solve();
    }

    public void Solve()
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
                    _global.A[rectangle.Elements[i], rectangle.Elements[j]] = localG[i, j] + localM[i, j];
                    
                }

                _global.F[rectangle.Elements[i]] += localB[i];
            }
        }
        
        Console.WriteLine("hghgh");
    }
}