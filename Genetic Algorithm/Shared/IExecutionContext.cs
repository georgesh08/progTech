using Shared.Models;

namespace Shared
{
    public interface IExecutionContext
    {
        void Reset();
        Task<IterationResult> ExecuteIterationAsync();
        void ReportStatistics(IStatisticsConsumer statisticsConsumer);
    }
}