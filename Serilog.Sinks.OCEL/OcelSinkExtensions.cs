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
            var ocelSink = new OcelLiteDbSink(options.Directory, options.FileName, options.RollingPeriod, options.Password);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options);
            return configuration.Sink(batchingSink);
        }

        public static LoggerConfiguration OcelXmlSink(
            this LoggerSinkConfiguration configuration,
            OcelXmlSinkOptions options)
        {
            var ocelSink = new OcelXmlSink(options.Directory, options.FileName, options.RollingPeriod, options.Formatting);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options);
            return configuration.Sink(batchingSink);
        }

        public static LoggerConfiguration OcelJsonSink(
            this LoggerSinkConfiguration configuration,
            OcelJsonSinkOptions options)
        {
            var ocelSink = new OcelJsonSink(options.Directory, options.FileName, options.RollingPeriod, options.Formatting);
            var batchingSink = new PeriodicBatchingSink(ocelSink, options);
            return configuration.Sink(batchingSink);
        }
    }

    public class LiteDbSinkOptions : PeriodicBatchingSinkOptions
    {
        public LiteDbSinkOptions(string directory, string fileName, RollingPeriod rollingPeriod, string password = null)
        {
            Directory = directory;
            FileName = fileName;
            RollingPeriod = rollingPeriod;
            Password = password;
        }

        /// <summary>
        /// The directory in which to store the log files.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The file name of a log file, which is prepended with the rolling period identifier.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The period after which a new log file is created.
        /// </summary>
        public RollingPeriod RollingPeriod { get; set; }

        /// <summary>
        /// An optional password for the log file (see <see href="https://www.litedb.org/docs/connection-string/">documentation</see>)
        /// </summary>
        public string Password { get; set; }
    }

    public class OcelJsonSinkOptions : PeriodicBatchingSinkOptions
    {
        public OcelJsonSinkOptions(string directory, string fileName, RollingPeriod rollingPeriod, global::OCEL.Types.Formatting formatting)
        {
            Directory = directory;
            FileName = fileName;
            RollingPeriod = rollingPeriod;
            Formatting = formatting;
        }

        /// <summary>
        /// The directory in which to store the log files.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The file name of a log file, which is prepended with the rolling period identifier.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The period after which a new log file is created.
        /// </summary>
        public RollingPeriod RollingPeriod { get; set; }

        /// <summary>
        /// The formatting to use,
        /// </summary>
        public global::OCEL.Types.Formatting Formatting { get; set; }
    }

    public class OcelXmlSinkOptions : PeriodicBatchingSinkOptions
    {
        public OcelXmlSinkOptions(string directory, string fileName, RollingPeriod rollingPeriod, global::OCEL.Types.Formatting formatting)
        {
            Directory = directory;
            FileName = fileName;
            RollingPeriod = rollingPeriod;
            Formatting = formatting;
        }

        /// <summary>
        /// The directory in which to store the log files.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The file name of a log file, which is prepended with the rolling period identifier.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The period after which a new log file is created.
        /// </summary>
        public RollingPeriod RollingPeriod { get; set; }

        /// <summary>
        /// The formatting to use,
        /// </summary>
        public global::OCEL.Types.Formatting Formatting { get; set; }
    }
}
