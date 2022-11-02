using System;
using LiteDB;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL.Sinks
{
    public class OcelLiteDbSink : IBatchedLogEventSink
    {
        private readonly string _connectionString;
        private readonly ITextFormatter _formatter;

        public OcelLiteDbSink(
            string connectionString,
            ITextFormatter formatter)
        {
            _connectionString = connectionString;
            _formatter = formatter;
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var items = batch.Select(e => AsDocument(e, _formatter));
            using var db = new LiteDatabase(_connectionString);
            ILiteCollection<BsonDocument> collection = db.GetCollection("log");
            collection.Insert(items);

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
