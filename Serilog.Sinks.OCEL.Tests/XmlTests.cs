using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.OCEL.Tests
{
    public class XmlTests
    {
        public XmlTests()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .MinimumLevel.Information()
                .WriteTo.OcelXmlSink("log.xmlocel")
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
                        {"date now", DateTime.Now}
                    }}
                });
            Log.Error(new ArgumentOutOfRangeException("some param", "test exception"), "test with error");
            Log.CloseAndFlush();
        }
    }
}
