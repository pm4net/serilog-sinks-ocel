using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.OCEL
{
    /// <summary>Defines when to use a new log file, based on the current system time.</summary>
    public enum RollingPeriod
    {
        /// <summary>Uses the file name as is.</summary>
        Never,
        /// <summary>Prepends yyyy_ to the file name.</summary>
        Year,
        /// <summary>Prepends yyyy-MM_ to the file name.</summary>
        Month,
        /// <summary>Prepends yyyy-MM-dd_ to the file name.</summary>
        Day,
        /// <summary>Prepends weekNumber_ to the file name, using the current culture's calendar.</summary>
        Week,
        /// <summary>Prepends yyyy-MM-dd_HH_ to the file name.</summary>
        Hour,
        /// <summary>Prepends yyyy-MM-dd_HH-mm_ to the file name, where mm is either 00 or 30.</summary>
        HalfHour,
        /// <summary>Prepends yyyy-MM-dd_HH-mm_ to the file name, where mm is 00, 15, 30, or 45.</summary>
        QuarterHour,
        /// <summary>Prepends yyyy-MM-dd_HH-mm_ to the file name.</summary>
        Minute
    }
}
