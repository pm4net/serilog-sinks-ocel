using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OCEL.CSharp;

namespace Serilog.Sinks.OCEL
{
    internal static class Helpers
    {
        internal static OcelLog MergeWith(this OcelLog log, OcelLog other)
        {
            log.Events = log.Events.Concat(other.Events).ToDictionary(x => x.Key, x => x.Value);
            log.Objects = log.Objects.Concat(other.Objects).ToDictionary(x => x.Key, x => x.Value);
            log.GlobalAttributes = log.GlobalAttributes.Concat(other.GlobalAttributes).ToDictionary(x => x.Key, x => x.Value);
            return log;
        }
    }
}
