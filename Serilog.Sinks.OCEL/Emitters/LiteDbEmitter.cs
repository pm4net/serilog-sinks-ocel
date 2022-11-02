using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.OCEL.Emitters
{
    public class LiteDbEmitter : Emitter
    {
        public LiteDbEmitter(string filePath, ITextFormatter formatter) : base(filePath, formatter)
        {
        }

        public override Task EmitBatch(IEnumerable<LogEvent> batch)
        {
            var items = batch.Select(e => AsDocument(e, Formatter));
            using var db = new LiteDatabase(FilePath);
            ILiteCollection<BsonDocument> collection = db.GetCollection("log");
            collection.Insert(items);
            return Task.CompletedTask;
        }

        private static BsonDocument AsDocument(LogEvent @event, ITextFormatter formatter)
        {
            var sw = new StringWriter();
            formatter.Format(@event, sw);
            return JsonSerializer.Deserialize(new StringReader(sw.ToString())).AsDocument;
        }
    }
}
