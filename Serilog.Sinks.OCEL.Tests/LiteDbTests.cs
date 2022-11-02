using LiteDB;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.OCEL.Tests
{
    public class LiteDbTests : IDisposable
    {
        private static readonly string FilePath = "../../../unit-tests.db";
        private static readonly string LiteDbConnection = $"Filename={FilePath};";
        private static readonly string InMemoryConnection = "Filename=:memory:;";

        private readonly Logger _logger;
        private readonly LiteDatabase _db;

        public LiteDbTests()
        {
            _db = new LiteDatabase(LiteDbConnection);
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.OcelLiteDbSink(LiteDbConnection, new JsonFormatter())
                .CreateLogger();
        }

        public void Dispose()
        {
            _db.Dispose();
            _logger.Dispose();

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        [Fact]
        public void CanWriteToDatabase()
        {
            _logger.Information("Test Message to Check Write Capability");
            Thread.Sleep(3_000); // Wait until all batches are written
            Assert.Equal(1, _db.GetCollection("log").Count());
        }
    }
}