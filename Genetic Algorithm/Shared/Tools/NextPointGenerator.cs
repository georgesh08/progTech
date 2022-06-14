using Shared.Models;

namespace Shared.Tools;

public static class NextPointGenerator
{
    private const double coeff = 0.09;
    private static Random random = new();
    public static Point GetNextPoint(Point prev, double forceVector)
    {
        var newX = prev.X + random.NextDouble() * forceVector * coeff;
        var newY = prev.Y + random.NextDouble() * forceVector * coeff;
        return new Point(newX, newY);
    }
}