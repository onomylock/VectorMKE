using Flee.PublicTypes;
using VectorMKE.Enums;
using VectorMKE.Models;

namespace VectorMKE.Services;

public interface IExpressionEvaluatorService
{
    double GetFuncValue(FunctionType functionType, double x = 0, double y = 0);
}

public class ExpressionEvaluatorService: IExpressionEvaluatorService
{
    private readonly ExpressionContext _expressionContext;
    private Dictionary<FunctionType ,IGenericExpression<double>> _genericExpressionDictionary;
    
    public ExpressionEvaluatorService(Dictionary<FunctionType, string> dictionaryOptions)
    {
        _expressionContext = new ExpressionContext();
        _expressionContext.Imports.AddType(typeof(Math));
        _genericExpressionDictionary = new Dictionary<FunctionType, IGenericExpression<double>>();
        foreach (var val in dictionaryOptions)
        {
            _genericExpressionDictionary.Add(val.Key, _expressionContext.CompileGeneric<double>(val.Value));    
        }
    }

    private void SetGenericExpression(FunctionType functionType,string func)
    {
        
    }
    
    public double GetFuncValue(FunctionType functionType ,double x = 0, double y = 0)
    {
        _expressionContext.Variables["x"] = x;
        _expressionContext.Variables["y"] = y;
        return _genericExpressionDictionary[functionType].Evaluate();
    }
}