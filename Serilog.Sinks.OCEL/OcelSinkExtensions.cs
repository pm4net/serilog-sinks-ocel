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
        public static LoggerConfiguration OcelLiteDbSink(
            this LoggerSinkConfiguration configuration,
            LiteDbSinkOptions options)
        {
            var ocelSink = new OcelLiteDbSink(options.ConnectionString);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options);
            return configuration.Sink(batchingSink);
        }

        public static LoggerConfiguration OcelXmlSink(
            this LoggerSinkConfiguration configuration,
            OcelXmlSinkOptions options)
        {
            var ocelSink = new OcelXmlSink(options.FilePath, options.Formatting);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options);
            return configuration.Sink(batchingSink);
        }

        public static LoggerConfiguration OcelJsonSink(
            this LoggerSinkConfiguration configuration,
            OcelJsonSinkOptions options)
        {
            var ocelSink = new OcelJsonSink(options.FilePath, options.Formatting);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options);
            return configuration.Sink(batchingSink);
        }
    }

    public class LiteDbSinkOptions : PeriodicBatchingSinkOptions
    {
        public LiteDbSinkOptions(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }

    public class OcelJsonSinkOptions : PeriodicBatchingSinkOptions
    {
        public OcelJsonSinkOptions(string filePath, global::OCEL.Types.Formatting formatting)
        {
            FilePath = filePath;
            Formatting = formatting;
        }

        public string FilePath { get; set; }

        public global::OCEL.Types.Formatting Formatting { get; set; }
    }

    public class OcelXmlSinkOptions : PeriodicBatchingSinkOptions
    {
        public OcelXmlSinkOptions(string filePath, global::OCEL.Types.Formatting formatting)
        {
            FilePath = filePath;
            Formatting = formatting;
        }

        public string FilePath { get; set; }

        public global::OCEL.Types.Formatting Formatting { get; set; }
    }
}
