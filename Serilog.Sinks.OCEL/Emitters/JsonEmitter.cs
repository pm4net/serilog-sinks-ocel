using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.OCEL.Emitters
{
    public class JsonEmitter : Emitter
    {
        public JsonEmitter(string filePath, ITextFormatter formatter) : base(filePath, formatter)
        {
        }

        public override Task EmitBatch(IEnumerable<LogEvent> batch)
        {
            throw new NotImplementedException();
        }
    }
}
