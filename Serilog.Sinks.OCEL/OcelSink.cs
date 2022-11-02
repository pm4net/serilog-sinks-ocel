using LiteDB;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.OCEL.Emitters;
using Serilog.Sinks.OCEL.Enums;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL
{
    public class OcelSink : IBatchedLogEventSink
    {
        private readonly OutputFormat _format;
        private readonly string _filePath;
        private readonly ITextFormatter _formatter;

        public OcelSink(
            OutputFormat format,
            string filePath,
            ITextFormatter formatter)
        {
            _format = format;
            _filePath = filePath;
            _formatter = formatter;
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            Emitter emitter = _format switch
            {
                OutputFormat.LiteDb => new LiteDbEmitter(_filePath, _formatter),
                OutputFormat.Xml => new XmlEmitter(_filePath, _formatter),
                OutputFormat.Json => new JsonEmitter(_filePath, _formatter),
                _ => throw new ArgumentOutOfRangeException(nameof(_format))
            };

            await emitter.EmitBatch(batch);
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}