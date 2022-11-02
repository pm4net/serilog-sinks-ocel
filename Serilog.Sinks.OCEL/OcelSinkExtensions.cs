using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Sinks.OCEL.Enums;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL
{
    public static class OcelSinkExtensions
    {
        public static LoggerConfiguration OcelSink(
            this LoggerSinkConfiguration configuration,
            OutputFormat format,
            string connectionString,
            ITextFormatter textFormatter)
        {
            var ocelSink = new OcelSink(format, connectionString, textFormatter);
            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = 50,
                EagerlyEmitFirstEvent = true,
                Period = TimeSpan.FromSeconds(2),
                QueueLimit = 100_000
            };
            var batchingSink = new PeriodicBatchingSink(ocelSink, batchingOptions);
            return configuration.Sink(batchingSink);
        }
    }
}
