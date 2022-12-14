using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using OCEL.CSharp;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.OCEL.Sinks
{
    public class OcelLiteDbSink : IBatchedLogEventSink
    {
        private readonly string _directory;

        private readonly string _fileName;

        private readonly RollingPeriod _rollingPeriod;

        private readonly string _password;

        public OcelLiteDbSink(string directory, string fileName, RollingPeriod rollingPeriod, string password = null)
        {
            _directory = directory;
            _fileName = fileName;
            _rollingPeriod = rollingPeriod;
            _password = password;
        }

        public Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            var connString = $"Filename={Helpers.DetermineFilePath(_directory, _fileName, _rollingPeriod)}";
            if (_password != null)
            {
                connString += $";Password={_password}";
            }

            using (var db = new LiteDatabase(connString))
            {
                OcelLiteDB.Serialize(db, batch.MapFromEvents());
                db.Dispose(); // https://github.com/mbdavid/LiteDB/issues/1462
            }
            return Task.CompletedTask;
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }
    }
}
