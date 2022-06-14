using Serilog;

namespace Shared;

public class Logger
{
    public static void Init()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("gen-algo.log", rollingInterval:RollingInterval.Day)
            .CreateLogger();
    }
}