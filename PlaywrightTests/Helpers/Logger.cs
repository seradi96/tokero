using Serilog;

namespace PlaywrightTests.Helpers
{
    public static class Logger
    {
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console() // Logs to the console
                .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), "logs/test-log.json", rollingInterval: RollingInterval.Day) // Logs to a JSON file
                .CreateLogger();
        }

        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }
    }
}