using Flee.PublicTypes;

namespace VectorMKE.Models;

public class Function(IGenericExpression<double> genericExpression)
{
    private readonly IGenericExpression<double> _genericExpression = genericExpression;
    
    
}