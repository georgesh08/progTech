namespace Shared.Models;

public class FitnessFunction
{
    public static double CountFitness(Point point)
    {
        var xDiff = 1.0 - point.X;
        var yDiff = 1.0 - point.Y;
        return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
    }
}