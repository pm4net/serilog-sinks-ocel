using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                .MinimumLevel.Information()
                .WriteTo.OcelXmlSink(new OcelXmlSinkOptions(@"log.xmlocel"))
                .CreateLogger();
        }

        [Fact]
        public void CanWriteToJsonFile()
        {
            Log.Information("Test message: {msg}, {msg2}, {msg3}, {msg4}, {msg5}, {msg6} and test object {@s}",
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
            Log.Error(new ArgumentOutOfRangeException("some param", "test exception"), "test with error");
            Log.CloseAndFlush();
        }

        [Fact]
        public void CanWriteManyLogs()
        {
            for (int i = 0; i < 100_000; i++)
            {
                Log.Information("Testing {emoji}", ":)");
            }
            Log.CloseAndFlush();
        }
    }
}
