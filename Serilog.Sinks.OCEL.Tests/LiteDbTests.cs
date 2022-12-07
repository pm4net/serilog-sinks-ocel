using LiteDB;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.OCEL.Sinks;

namespace Serilog.Sinks.OCEL.Tests
{
    public class LiteDbTests
    {
        private const string FilePath = "unit-tests.db";
        private const string LiteDbConnection = $"Filename={FilePath};";
        private const string InMemoryConnection = "Filename=:memory:;";

        public LiteDbTests()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .MinimumLevel.Information()
                .WriteTo.OcelLiteDbSink(LiteDbConnection)
                .CreateLogger();
        }

        [Fact]
        public void CanWriteToDatabase()
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