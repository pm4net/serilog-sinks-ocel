using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCEL.CSharp;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.OCEL
{
    internal static class EventMapper
    {
        private static OcelValue MapScalarValue(this ScalarValue scalarValue)
        {
            switch (scalarValue.Value)
            {
                // Integer types
                case sbyte sby:
                    return new OcelInteger(sby);
                case byte by:
                    return new OcelInteger(by);
                case short sh:
                    return new OcelInteger(sh);
                case ushort ush:
                    return new OcelInteger(ush);
                case int i:
                    return new OcelInteger(i);
                case uint ui:
                    return new OcelInteger(ui);
                case long l:
                    return new OcelInteger(l);
                case ulong ul:
                    return new OcelInteger((long)ul);

                // Floating-point types
                case float f:
                    return new OcelFloat(f);
                case double d:
                    return new OcelFloat(d);
                case decimal dec:
                    return new OcelFloat((double)dec);

                // Boolean types
                case bool b:
                    return new OcelBoolean(b);

                // Date times
                case DateTimeOffset dto:
                    return new OcelTimestamp(dto);
                case DateTime dt:
                    return new OcelTimestamp(dt);

                // Other types
                case char ch:
                    return new OcelString(new string(ch, 1));
                default:
                    return new OcelString(scalarValue.Value?.ToString() ?? string.Empty);
            }
        }

        internal static OcelLog MapFromEvents(this IEnumerable<LogEvent> events)
        {
            var log = new OcelLog(new Dictionary<string, OcelValue>(), new Dictionary<string, OcelEvent>(), new Dictionary<string, OcelObject>());

            foreach (var @event in events)
            {
                var vMap = new Dictionary<string, OcelValue>();
                var oMap = new Dictionary<string, OcelObject>();

                // Add log level as an attribute
                vMap["Level"] = new OcelString(@event.Level.ToString());

                foreach (var property in @event.Properties)
                {
                    switch (property.Value)
                    {
                        case ScalarValue scalarValue:
                            vMap[property.Key] = scalarValue.MapScalarValue();
                            break;
                        case StructureValue structureValue:
                            // TODO: Pending OCEL list/mapping implementation
                            break;
                        case SequenceValue sequenceValue:
                            // TODO: Pending OCEL list/mapping implementation
                            break;
                        case DictionaryValue dictionaryValue:
                            // TODO: Pending OCEL list/mapping implementation
                            break;
                        default:
                            throw new ArgumentException($"Property type {property.Value.GetType()} on property {property.Key} not supported");
                    }
                }

                // Add exception as an object
                if (@event.Exception != null)
                {
                    // TODO: Pending OCEL list/mapping implementation
                }

                var ocelEvent = new OcelEvent(
                    activity: @event.MessageTemplate.Text, 
                    timestamp: @event.Timestamp, 
                    oMap: oMap.Keys,
                    vMap: vMap);

                log.Events.Add(Guid.NewGuid().ToString(), ocelEvent);
            }

            return log;
        }
    }
}
