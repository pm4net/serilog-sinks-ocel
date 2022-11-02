using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.OCEL.Emitters
{
    public abstract class Emitter
    {
        protected readonly string FilePath;
        protected readonly ITextFormatter Formatter;

        protected Emitter(string filePath, ITextFormatter formatter)
        {
            FilePath = filePath;
            Formatter = formatter;
        }

        public abstract Task EmitBatch(IEnumerable<LogEvent> batch);
    }
}
