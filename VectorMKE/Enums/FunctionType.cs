using System.Text.Json.Serialization;

namespace VectorMKE.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FunctionType
{
    FunX,
    FunY,
    Fun,
    Mu,
    Gamma,
    Tetta
}