using Flee.PublicTypes;
using VectorMKE.Enums;

namespace VectorMKE.Services;

public interface IFunctionService
{
    double GetFuncValue(FunctionType functionType, double x = 0, double y = 0);
}

public class FunctionService: IFunctionService
{
    private readonly ExpressionContext _expressionContext;
    private Dictionary<FunctionType ,IGenericExpression<double>> _genericExpressionDictionary;
    
    public FunctionService(Dictionary<FunctionType, string> dictionaryOptions)
    {
        _expressionContext = new ExpressionContext();
        _expressionContext.Imports.AddType(typeof(Math));
        _expressionContext.Variables.Add("x", 0.0);
        _expressionContext.Variables.Add("y", 0.0);
        _genericExpressionDictionary = new Dictionary<FunctionType, IGenericExpression<double>>();
        foreach (var val in dictionaryOptions)
        {
            _genericExpressionDictionary.Add(val.Key, _expressionContext.CompileGeneric<double>(val.Value));    
        }
    }

    public double GetFuncValue(FunctionType functionType ,double x = 0, double y = 0)
    {
        _expressionContext.Variables["x"] = x;
        _expressionContext.Variables["y"] = y;
        var result = _genericExpressionDictionary[functionType].Evaluate();
        return result;
    }
}