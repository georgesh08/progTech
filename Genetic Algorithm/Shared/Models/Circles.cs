using System.Runtime.CompilerServices;

namespace Shared.Models;

public class Circles
{
    private List<BarrierCircle> _circles = new(){
        new BarrierCircle(new Point(0.2, 0.3), 20),
        new BarrierCircle(new Point(0.5, 0.2), 20),
        new BarrierCircle(new Point(0.8, 0.5), 40),
        new BarrierCircle(new Point(0.2, 0.8), 40)
    };

    public IReadOnlyList<BarrierCircle> CirclesData()
    {
        return _circles;
    }

    public void SetNewCircles(List<BarrierCircle> circles)
    {
        _circles = circles;
    }
}