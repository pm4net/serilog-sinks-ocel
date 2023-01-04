﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public OcelXmlSink(string directory, string fileName, RollingPeriod rollingPeriod, global::OCEL.Types.Formatting formatting)
        {
            _directory = directory;
            _fileName = fileName;
            _rollingPeriod = rollingPeriod;
            _formatting = formatting;
        }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")] // Not available in .NET Standard 2.0
        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var file = Helpers.DetermineFilePath(_directory, _fileName, _rollingPeriod);
            var newLog = batch.MapFromEvents();
            if (File.Exists(file))
            {
                var xml = File.ReadAllText(file);
                var log = OcelXml.Deserialize(xml);
                newLog = log.MergeWith(newLog);
            }

            var serialized = OcelXml.Serialize(newLog, _formatting);
            File.WriteAllText(file, serialized);
            return Task.CompletedTask;
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
