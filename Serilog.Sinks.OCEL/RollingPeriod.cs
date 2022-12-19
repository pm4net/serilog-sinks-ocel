using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.OCEL
{
    public enum RollingPeriod
    {
        Never,
        Year,
        Month,
        Day,
        Week,
        Hour,
        HalfHour,
        QuarterHour,
        Minute
    }
}
