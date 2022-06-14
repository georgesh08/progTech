using Shared.Models;

namespace Shared;

public interface IStatisticsConsumer
{
    void Consume(IReadOnlyCollection<Statistic> statistics, IReadOnlyCollection<BarrierCircle> barriers);
}