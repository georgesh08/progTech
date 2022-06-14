namespace Shared.Models;

public class Individual
{
    private Point _currentPos;
    private double _distance = Math.Sqrt(2);

    public Individual()
    {
        _currentPos = new Point(0, 0);
    }

    public void Reset()
    {
        _currentPos.X = 0;
        _currentPos.Y = 0;
    }

    public Point CurrentPosition()
    {
        return _currentPos;
    }

    public void SetPosition(Point newPoint)
    {
        _currentPos = newPoint;
    }

    public double Distance()
    {
        return _distance;
    }

    public void SetDistance(double newDist)
    {
        _distance = newDist;
    }

    public bool IsAtStart()
    {
        return _currentPos.X == 0 & _currentPos.Y == 0;
    }
}