using VectorMKE.Services;

namespace VectorMKE;

public static class Pragma
{
    public static void Main()
    {
        var solver = new SolverService("Settings\\Options2D.json");
        if (solver == null) throw new ArgumentNullException(nameof(solver));
    }
}

