using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OCEL.CSharp;
using Serilog.Events;

namespace Serilog.Sinks.OCEL
{
    internal static class EventMapper
    {
        internal static OcelLog MapFromEvents(this IEnumerable<LogEvent> events, string prefix)
        {
            var log = new OcelLog(new Dictionary<string, OcelValue>(), new Dictionary<string, OcelEvent>(), new Dictionary<string, OcelObject>());

            foreach (var @event in events)
            {
                var vMap = new Dictionary<string, OcelValue>();
                var objectIds = new List<string>();

                // Add basic information as attributes
                vMap[$"{prefix}Level"] = new OcelString(@event.Level.ToString());
                vMap[$"{prefix}RenderedMessage"] = new OcelString(@event.RenderMessage());

                // Add exception as an attribute
                if (@event.Exception != null)
                {
                    vMap[$"{prefix}Exception"] = MapException(@event.Exception);
                }

                // Partition the properties by whether they start with the reserved prefix
                var lookup = @event.Properties.ToLookup(x => x.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || x.Key == "SourceContext");

                // Add properties that start with the reserved prefix as attributes
                foreach (var property in lookup[true])
                {
                    vMap[property.Key] = MapLogEventPropertyValue(property.Value);
                }

                // Add remaining properties as objects
                foreach (var property in lookup[false])
                {
                    var objectId = Guid.NewGuid().ToString();
                    objectIds.Add(objectId);

                    switch (property.Value)
                    {
                        case StructureValue structureValue:
                            log.Objects.Add(objectId, new OcelObject(property.Key, 
                                structureValue.Properties.ToDictionary(x => x.Name, x => MapLogEventPropertyValue(x.Value))));
                            break;
                        case DictionaryValue dictionaryValue:
                            log.Objects.Add(objectId, new OcelObject(property.Key,
                                dictionaryValue.Elements.ToDictionary(
                                    x => x.Key.Value as string ?? Guid.NewGuid().ToString(),
                                    x => MapLogEventPropertyValue(x.Value))));
                            break;
                        default:
                            log.Objects.Add(objectId, new OcelObject(property.Key, new Dictionary<string, OcelValue>
                            {
                                { "value", MapLogEventPropertyValue(property.Value) }
                            }));
                            break;
                    }
                }
                
                var ocelEvent = new OcelEvent(
                    activity: @event.MessageTemplate.Text, 
                    timestamp: @event.Timestamp, 
                    oMap: objectIds,
                    vMap: vMap);

                log.Events.Add(Guid.NewGuid().ToString(), ocelEvent);
            }

            return log;
        }

        /// <summary>
        /// Map an Exception to an OCEL map value with its details as children.
        /// </summary>
        /// <param name="ex">The exception to map</param>
        /// <returns>An OCEL map with the exception's details</returns>
        private static OcelValue MapException(Exception ex)
        {
            var values = new Dictionary<string, OcelValue>
            {
                ["Type"] = new OcelString(ex.GetType().FullName),
                ["Message"] = new OcelString(ex.Message),
                ["HResult"] = new OcelInteger(ex.HResult)
            };

            if (ex.StackTrace != null)
            {
                values["StackTrace"] = new OcelString(ex.StackTrace);
            }

            if (ex.Source != null)
            {
                values["Source"] = new OcelString(ex.Source);
            }

            if (ex.HelpLink != null)
            {
                values["HelpLink"] = new OcelString(ex.HelpLink);
            }

            if (ex.TargetSite != null)
            {
                values["TargetSite"] = new OcelString(ex.TargetSite.ToString());
            }

            foreach (DictionaryEntry entry in ex.Data)
            {
                var key = entry.Key.ToString();
                if (!string.IsNullOrWhiteSpace(key) && entry.Value != null)
                {
                    values[key] = MapObject(entry.Value);
                }
            }

            return new OcelMap(values);
        }

        /// <summary>
        /// Convert any object to an OCEL value, finding the most suitable type based on its .NET type.
        /// Falls back to string representation if no type matches (using .ToString()).
        /// </summary>
        /// <param name="obj">The object to map</param>
        /// <returns>An OCEL value that represents the object.</returns>
        private static OcelValue MapObject(object obj)
        {
            switch (obj)
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
                case IEnumerable<object> en:
                    return new OcelList(en.Select(MapObject));
                case IDictionary<string, object> dict:
                    return new OcelMap(dict.ToDictionary(x => x.Key, x => MapObject(x.Value)));
                default:
                    return new OcelString(obj.ToString() ?? string.Empty);
            }
        }

        /// <summary>
        /// Map Serilogs various log event property value types to a fitting OCEL value.
        /// </summary>
        /// <param name="value">The log event property value</param>
        /// <returns>An OCEL value that represents the value.</returns>
        /// <exception cref="ArgumentException">If the value is not one of known types</exception>
        private static OcelValue MapLogEventPropertyValue(LogEventPropertyValue value)
        {
            switch (value)
            {
                case ScalarValue scalarValue:
                    return scalarValue.Value != null ? MapObject(scalarValue.Value) : null;
                case StructureValue structureValue:
                    return new OcelMap(structureValue.Properties.ToDictionary(x => x.Name, x => MapLogEventPropertyValue(x.Value)));
                case SequenceValue sequenceValue:
                    return new OcelList(sequenceValue.Elements.Select(MapLogEventPropertyValue));
                case DictionaryValue dictionaryValue:
                    return new OcelMap(dictionaryValue.Elements.ToDictionary(
                        x => x.Key.Value as string ?? Guid.NewGuid().ToString(),
                        x => MapLogEventPropertyValue(x.Value)));
                default:
                    throw new ArgumentException($"Property type {value.GetType()} not supported here.");
            }
        }
    }
}
