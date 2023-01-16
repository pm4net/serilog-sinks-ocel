using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.FSharp.Control;
using OCEL.CSharp;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.OCEL
{
    internal static class EventMapper
    {
        private const string Prefix = "pm4net";

        internal static OcelLog MapFromEvents(this IEnumerable<LogEvent> events)
        {
            var log = new OcelLog(new Dictionary<string, OcelValue>(), new Dictionary<string, OcelEvent>(), new Dictionary<string, OcelObject>());

            foreach (var @event in events)
            {
                var vMap = new Dictionary<string, OcelValue>();
                var objectIds = new List<string>();

                // Add basic information as attributes
                vMap[$"{Prefix}_Level"] = new OcelString(@event.Level.ToString());
                vMap[$"{Prefix}_RenderedMessage"] = new OcelString(@event.RenderMessage());

                // Partition the properties by whether they start with the reserved prefix
                var lookup = @event.Properties.ToLookup(x => x.Key.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase));

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

                // Add exception as an object
                if (@event.Exception != null)
                {
                    var exObj = MapException(@event.Exception);
                    var objectId = Guid.NewGuid().ToString();
                    objectIds.Add(objectId);
                    log.Objects[objectId] = exObj;
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
        /// Map an Exception to an OCEL object with its details as attributes.
        /// </summary>
        /// <param name="ex">The exception to map</param>
        /// <returns>An OCEL object with the exception's details</returns>
        private static OcelObject MapException(Exception ex)
        {
            var exObj = new OcelObject($"{Prefix}.Exception", new Dictionary<string, OcelValue>());
            exObj.OvMap["Message"] = new OcelString(ex.Message);
            exObj.OvMap["HResult"] = new OcelInteger(ex.HResult);

            if (ex.StackTrace != null)
            {
                exObj.OvMap["StackTrace"] = new OcelString(ex.StackTrace);
            }

            if (ex.Source != null)
            {
                exObj.OvMap["Source"] = new OcelString(ex.Source);
            }

            if (ex.HelpLink != null)
            {
                exObj.OvMap["HelpLink"] = new OcelString(ex.HelpLink);
            }

            if (ex.TargetSite != null)
            {
                exObj.OvMap["TargetSite"] = new OcelString(ex.TargetSite.ToString());
            }

            foreach (DictionaryEntry entry in ex.Data)
            {
                var key = entry.Key.ToString();
                if (!string.IsNullOrWhiteSpace(key) && entry.Value != null)
                {
                    exObj.OvMap[key] = MapObject(entry.Value);
                }
            }

            return exObj;
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
