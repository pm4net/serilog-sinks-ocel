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
    public class OcelXmlSink : IBatchedLogEventSink
    {
        private readonly string _filePath;

        public OcelXmlSink(string filePath)
        {
            _filePath = filePath;
        }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")] // Not available in .NET Standard 2.0
        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var newLog = batch.MapFromEvents();
            if (File.Exists(_filePath))
            {
                var xml = File.ReadAllText(_filePath);
                var log = OcelXml.Deserialize(xml);
                newLog = log.MergeWith(newLog);
            }

            File.WriteAllText(_filePath, OcelXml.Serialize(newLog, global::OCEL.Types.Formatting.Indented));
            return Task.CompletedTask;
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
