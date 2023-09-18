using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OCEL.CSharp;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL.Sinks
{
    public class OcelJsonSink : IBatchedLogEventSink
    {
        private readonly string _directory;

        private readonly string _fileName;

        private readonly RollingPeriod _rollingPeriod;

        private readonly global::OCEL.Types.Formatting _formatting;

        private readonly string _prefix = "pm4net_";

        public OcelJsonSink(string directory, string fileName, RollingPeriod rollingPeriod, global::OCEL.Types.Formatting formatting, string prefix = null)
        {
            _directory = directory;
            _fileName = fileName;
            _rollingPeriod = rollingPeriod;
            _formatting = formatting;

            if (prefix != null)
            {
                _prefix = prefix;
            }
        }

        /// <summary>
        /// In-memory copy of serialized log to avoid re-reading from file all the time.
        /// </summary>
        private OcelLog _inMemoryLog;

        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var file = Helpers.DetermineFilePath(_directory, _fileName, _rollingPeriod);
            var fileDir = Path.GetDirectoryName(file);
            if (!string.IsNullOrWhiteSpace(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            var newLog = batch.MapFromEvents(_prefix);

            if (_inMemoryLog == null && File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var log = OcelJson.Deserialize(json, false);
                newLog = log.MergeWith(newLog);
            }
            else
            {
                newLog = _inMemoryLog != null ? _inMemoryLog.MergeWith(newLog) : newLog;
            }
            
            _inMemoryLog = newLog.MergeDuplicateObjects();
            var serialized = OcelJson.Serialize(_inMemoryLog, _formatting, false);
            File.WriteAllText(file, serialized);
            return Task.CompletedTask;
        }
        
        public Task OnEmptyBatchAsync()
        {
            _inMemoryLog = null;
            return Task.CompletedTask;
        }
    }
}
