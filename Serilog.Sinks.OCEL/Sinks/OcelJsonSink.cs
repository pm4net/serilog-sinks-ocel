﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            throw new NotImplementedException();
        }

        public Task OnEmptyBatchAsync()
        {
            throw new NotImplementedException();
        }
    }
}