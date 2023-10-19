using Serilog.Enrichers.CallerInfo;
using Xunit.Abstractions;

namespace Serilog.Sinks.OCEL.Tests
{
    public class XmlTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XmlTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithCallerInfo(includeFileInfo: true, allowedAssemblies: new List<string> { "Serilog.Sinks.OCEL.Tests" }, "pm4net_")
                .MinimumLevel.Information()
                .WriteTo.OcelXmlSink(new OcelXmlSinkOptions(string.Empty, "log.xmlocel", RollingPeriod.Never, global::OCEL.Types.Formatting.Indented))
                .CreateLogger();
        }

        [Fact]
        public void CanWriteToXmlFile()
        {
            Log.ForContext<XmlTests>().Information("Test message: {msg}, {msg2}, {msg3}, {msg4}, {msg5}, {msg6} and test object {@s}",
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
                });
            Log.ForContext<XmlTests>().Error(new ArgumentOutOfRangeException("some param", "test exception"), "test with error");
            Log.CloseAndFlush();
        }

        [Fact]
        public void CanWriteManyLogs()
        {
            for (int i = 0; i < 100; i++)
            {
                Log.Information("Testing {emoji}", ":)");
            }
            Log.CloseAndFlush();
        }
    }
}
