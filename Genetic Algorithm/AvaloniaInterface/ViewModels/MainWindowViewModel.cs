using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Shared;
using Shared.Models;
using UIShared;
using OxyPlot;
using OxyPlot.Series;

namespace AvaloniaInterface.ViewModels;

public class MainWindowViewModel : AvaloniaObject
{
    private readonly IStatisticsConsumer _statisticsConsumer;
    private readonly IExecutionContext _executionContext;
    private readonly ExecutionConfiguration _configuration;
    private readonly int _maxIteration = 100;

    public MainWindowViewModel(
        IExecutionContext executionContext,
        ExecutionConfiguration configuration
        )
    {
        _executionContext = executionContext;
        _configuration = configuration;

        IsRunning = AvaloniaProperty
            .RegisterAttached<MainWindowViewModel, bool>(nameof(IsRunning), typeof(MainWindowViewModel));

        var lineSeries = new ScatterSeries
        {
            MarkerSize = 1,
            MarkerStroke = OxyColors.ForestGreen,
            MarkerType = MarkerType.Circle,
        };

        var circleSeries = new ScatterSeries
        {
            MarkerFill = OxyColors.Red,
            MarkerType = MarkerType.Circle,
        };

        ScatterModel = new PlotModel
        {
            Title = "Points",
            Series = { circleSeries, lineSeries },
        };

        var barSeries = new LinearBarSeries
        {
            DataFieldX = nameof(FitnessModel.X),
            DataFieldY = nameof(FitnessModel.Y),
        };

        BarModel = new PlotModel
        {
            Title = "Fitness",
            Series = { barSeries },
        };

        _statisticsConsumer = new PlotStatisticConsumer(circleSeries, lineSeries, barSeries);
    }

    public PlotModel ScatterModel { get; }
    public PlotModel BarModel { get; }

    public AttachedProperty<bool> IsRunning { get; }

    public async Task RunAsync()
    {
        foreach (var series in ScatterModel.Series.OfType<XYAxisSeries>())
        {
            series.XAxis.Maximum = _configuration.MaximumValue;
            series.XAxis.Minimum = _configuration.MinimumValue;
            series.YAxis.Maximum = _configuration.MaximumValue;
            series.YAxis.Minimum = _configuration.MinimumValue;
        }

        SetValue(IsRunning, true);
        _executionContext.Reset();

        IterationResult iterationResult;

        int i = 0; 

        do
        {
            i++;
            iterationResult = await _executionContext.ExecuteIterationAsync();
            _executionContext.ReportStatistics(_statisticsConsumer);

            ScatterModel.InvalidatePlot(true);
            BarModel.InvalidatePlot(true);

            await Task.Delay(_configuration.IterationDelay);
        }
        while (i < _maxIteration & iterationResult == IterationResult.IterationFinished && GetValue(IsRunning) );
    }

    public void Stop()
    {
        SetValue(IsRunning, false);
    }
}