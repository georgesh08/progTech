namespace Shared.Models;

public record ExecutionConfiguration(TimeSpan IterationDelay, int MaximumValue, int MinimumValue);