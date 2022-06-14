using Shared.Models;

namespace Shared.Tools;

public class ExecutionContext : IExecutionContext
{
    private readonly int _pointCount;
    private List<Individual> _population = new();
    private readonly int _maximumValue;
    private readonly Circles _circles = new ();
    private readonly Random _random = new();
    private const double RadiusCoeff = 200;

    public ExecutionContext(int pointCount, int maximumValue)
    {
        for (var i = 0; i < pointCount; i++)
        {
            _population.Add(new Individual());
        }
        _pointCount = pointCount;
        _maximumValue = maximumValue;
    }
    
    public void Reset()
    {
        foreach (var individual in _population)
        {
            individual.Reset();
        }
    }

    public Task<IterationResult> ExecuteIterationAsync()
    {
        foreach (var individual in _population)
        {
            var nextPoint = NextPointGenerator.GetNextPoint(individual.CurrentPosition(), _maximumValue);
            
            if (!IsInCircle(nextPoint) & !IsOutOfBounds(nextPoint))
            {
                individual.SetPosition(nextPoint);
            }
            else
            {
                individual.Reset();
            }
        }
        
        MutatePopulation();

        return Task.FromResult(SolutionFound(GetCurrentPositions()) 
            ? IterationResult.SolutionFound 
            : IterationResult.IterationFinished);
    }

    private void MutatePopulation()
    {
        var nextPopulationSize = Convert.ToInt32(_pointCount * 0.8);
        var bestIndividualsSize = Convert.ToInt32(nextPopulationSize * 0.2);

        foreach (var individual in _population)
        {
            var distance = individual.IsAtStart() ? -1
                : FitnessFunction.CountFitness(individual.CurrentPosition());
            individual.SetDistance(distance);
        }
        
        _population = _population.OrderBy(i=>i.Distance()).ToList();

        var bestIndividualPosition = GetBestFromPopulation();
        
        for (var i = 0; i < _pointCount - nextPopulationSize; i++)
        {
            _population.ElementAt(i).SetPosition(bestIndividualPosition);
        }

        for (var i = _pointCount - bestIndividualsSize; i < _pointCount; i++)
        {
            MutateIndividual(_population.ElementAt(i));
        }
    }

    private Point GetBestFromPopulation()
    {
        foreach (var individual in _population.Where(individual => individual.Distance() > 0))
        {
            return individual.CurrentPosition();
        }

        return _population.Last().CurrentPosition();
    }
    
    private void MutateIndividual(Individual individual)
    {
        var newX = individual.CurrentPosition().X + _maximumValue * _random.NextDouble() * 0.01;
        var newY = individual.CurrentPosition().Y + _maximumValue * _random.NextDouble() * 0.01;
        individual.SetPosition(new Point(newX, newY));
    }

    public void ReportStatistics(IStatisticsConsumer statisticsConsumer)
    {
        Statistic[] statistics = Enumerable.Range(0, _pointCount)
            .Select(i => new Statistic(i, _population.ElementAt(i).CurrentPosition(), 
                FitnessFunction.CountFitness(_population.ElementAt(i).CurrentPosition())))
            .ToArray();

        statisticsConsumer.Consume(statistics, _circles.CirclesData());
    }

    private List<Point> GetCurrentPositions()
    {
        return _population.Select(individual => individual.CurrentPosition()).ToList();
    }

    private bool IsInCircle(Point point)
    {
        return _circles.CirclesData().Any(circle => 
            Math.Pow(point.X - circle.Center.X, 2) 
            + Math.Pow(point.Y - circle.Center.Y, 2) 
            <= Math.Pow(circle.Radius / RadiusCoeff, 2));
    }

    private bool IsOutOfBounds(Point point)
    {
        return point.X < 0 | point.X > 1 | point.Y < 0 | point.Y > 1;
    }

    private bool SolutionFound(IEnumerable<Point> points)
    {
        return points.Any(point => Math.Abs(point.X - 1) < 0.01 & Math.Abs(point.Y - 1) < 0.01);
    }
}