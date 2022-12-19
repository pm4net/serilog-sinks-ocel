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

        private readonly global::OCEL.Types.Formatting _formatting;

        public OcelJsonSink(string filePath, global::OCEL.Types.Formatting formatting)
        {
            _filePath = filePath;
            _formatting = formatting;
        }
        
        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")] // Not available in .NET Standard 2.0
        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var newLog = batch.MapFromEvents();
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var log = OcelJson.Deserialize(json);
                newLog = log.MergeWith(newLog);
            }
            
            File.WriteAllText(_filePath, OcelJson.Serialize(newLog, _formatting));
            return Task.CompletedTask;
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
