using LiteDB;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL
{
    public class OcelSink : IBatchedLogEventSink
    {
        private readonly string _connectionString;
        private readonly ITextFormatter _formatter;

        public OcelSink(string connectionString, ITextFormatter formatter)
        {
            _connectionString = connectionString;
            _formatter = formatter;
        }

        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            using var db = new LiteDatabase(_connectionString);
            ILiteCollection<BsonDocument> collection = db.GetCollection("log");
            collection.Insert(batch.Select(e => AsDocument(e, _formatter)));
            return Task.CompletedTask;

            BsonDocument AsDocument(LogEvent @event, ITextFormatter formatter)
            {
                var sw = new StringWriter();
                formatter.Format(@event, sw);
                return JsonSerializer.Deserialize(new StringReader(sw.ToString())).AsDocument;
            }
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}