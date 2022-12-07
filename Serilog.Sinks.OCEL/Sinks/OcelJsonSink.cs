using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCEL.CSharp;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL.Sinks
{
    public class OcelJsonSink : IBatchedLogEventSink
    {
        private readonly string _filePath;

        public OcelJsonSink(string filePath)
        {
            _filePath = filePath;
        }
        
        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")] // Not available in .NET Standard 2.0
        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var log = OcelJson.Deserialize(json);
            }

            var newLog = batch.MapFromEvents();
            // TODO: Merge old and new log together with library function (to be implemented)
            File.WriteAllText(_filePath, OcelJson.Serialize(newLog, global::OCEL.Types.Formatting.Indented));
            return Task.CompletedTask;
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
