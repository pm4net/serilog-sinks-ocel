using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OCEL.CSharp;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL.Sinks
{
    public class OcelXmlSink : IBatchedLogEventSink
    {
        private readonly string _directory;

        private readonly string _fileName;

        private readonly RollingPeriod _rollingPeriod;

        private readonly global::OCEL.Types.Formatting _formatting;

        private readonly string _prefix = "pm4net_";

        public OcelXmlSink(string directory, string fileName, RollingPeriod rollingPeriod, global::OCEL.Types.Formatting formatting, string prefix = null)
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
        
        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var file = Helpers.DetermineFilePath(_directory, _fileName, _rollingPeriod);
            var newLog = batch.MapFromEvents(_prefix);
            if (File.Exists(file))
            {
                var xml = File.ReadAllText(file);
                var log = OcelXml.Deserialize(xml);
                newLog = log.MergeWith(newLog).MergeDuplicateObjects();
            }

            var serialized = OcelXml.Serialize(newLog, _formatting);
            File.WriteAllText(file, serialized);
            return Task.CompletedTask;
        }

        // TODO: Keep log in memory so that it doesn't have to be read each time, and only dispose of it here. Then re-read it for the next time.
        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
