using Serilog.Enrichers.CallerInfo;
using Xunit.Abstractions;

namespace Serilog.Sinks.OCEL.Tests
{
    public class LiteDbTests
    {
        private const string FileName = "log.db";
        private const string InMemoryConnection = "Filename=:memory:;";

        private readonly ITestOutputHelper _testOutputHelper;

        public LiteDbTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithCallerInfo(includeFileInfo: true, allowedAssemblies: new List<string> { "Serilog.Sinks.OCEL.Tests" }, "pm4net_")
                .MinimumLevel.Information()
                .WriteTo.OcelLiteDbSink(new LiteDbSinkOptions(string.Empty, FileName, RollingPeriod.Never))
                .CreateLogger();
        }

        [Fact]
        public void CanWriteToDatabase()
        {
            Log.ForContext<LiteDbTests>().Information("Test message: {msg}, {msg2}, {msg3}, {msg4}, {msg5}, {msg6} and test object {@s}, and a {pm4net_Reserved}", 
                1337, 4.20, "test", false, DateTimeOffset.Now, new List<string> { "a", "b", "c" }, 
                new Dictionary<string, object>()
                {
                    { "a", 13 },
                    { "b", "test" },
                    { "c", new List<bool> { true, false, true }},
                    { "d", new Dictionary<string, DateTime>
                    {
                        {"datenow", DateTime.Now}
                    }}
                }, 1337);
            Log.ForContext<LiteDbTests>().Error(new ArgumentOutOfRangeException("some param", "test exception"), "test with error");
            Log.CloseAndFlush();
        }

        [Fact]
        public void CanWriteManyLogs()
        {
            for (int i = 0; i < 10_000; i++)
            {
                Log.Information("Testing {emoji}", ":)");
            }
            Log.CloseAndFlush();
        }
    }
}