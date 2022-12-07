using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OCEL.CSharp;
using LiteDB;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL.Sinks
{
    public class OcelLiteDbSink : IBatchedLogEventSink
    {
        private readonly string _connectionString;

        public OcelLiteDbSink(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                OcelLiteDB.Serialize(db, batch.MapFromEvents());
            }
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
