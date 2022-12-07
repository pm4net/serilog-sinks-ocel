using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Sinks.OCEL.Sinks;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL
{
    public static class OcelSinkExtensions
    {
        public static readonly PeriodicBatchingSinkOptions DefaultBatchingOptions = new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = 50,
            EagerlyEmitFirstEvent = true,
            Period = TimeSpan.FromSeconds(2),
            QueueLimit = 100_000
        };

        public static LoggerConfiguration OcelLiteDbSink(
            this LoggerSinkConfiguration configuration,
            string connectionString,
            PeriodicBatchingSinkOptions options)
        {
            var ocelSink = new OcelLiteDbSink(connectionString);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options ?? DefaultBatchingOptions);
            return configuration.Sink(batchingSink);
        }

        public static LoggerConfiguration OcelXmlSink(
            this LoggerSinkConfiguration configuration,
            string filePath,
            PeriodicBatchingSinkOptions options)
        {
            var ocelSink = new OcelXmlSink(filePath);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options ?? DefaultBatchingOptions);
            return configuration.Sink(batchingSink);
        }

        public static LoggerConfiguration OcelJsonSink(
            this LoggerSinkConfiguration configuration,
            string filePath,
            PeriodicBatchingSinkOptions options)
        {
            var ocelSink = new OcelJsonSink(filePath);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options ?? DefaultBatchingOptions);
            return configuration.Sink(batchingSink);
        }
    }
}
