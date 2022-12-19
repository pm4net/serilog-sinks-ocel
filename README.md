# Serilog.Sinks.OCEL

A Serilog sinks that writes events to Object-Centric Event Logs (OCEL) [1], using the [.NET OCEL library](https://github.com/pm4net/OCEL).

There is a separate sinks for each of the supported OCEL format: `Serilog.Sinks.OCEL.Sinks.OcelJsonSink`, `Serilog.Sinks.OCEL.Sinks.OcelXmlSink`, and `Serilog.Sinks.OCEL.Sinks.OcelLiteDbSink`.

Rolling files are supported by configuring the log directory and file name separately. Depending on the [rolling period](https://github.com/pm4net/serilog-sinks-ocel/blob/master/Serilog.Sinks.OCEL/RollingPeriod.cs), the file name is prepended with an identifier.

Sample configuration:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.OcelJsonSink(new OcelJsonSinkOptions(string.Empty, "log.jsonocel", RollingPeriod.Never, global::OCEL.Types.Formatting.Indented))
    .CreateLogger();
```

# Supported formats

The OCEL standard is defined for both JSON and XML. Both include a validation schema that is used by the library to validate input.

An additional useful format is to store OCEL data in document databases such as MongoDB [2]. A very good alternative for .NET is [LiteDB](https://www.litedb.org/), which is an embedded NoSQL database that is similar to MongoDB. It allows writing to files directly and does not require a database server to use. Support for MongoDB will be evaluated in the future.

| Format        | Status        |
| ------------- |:-------------:|
| JSON          | Implemented   |
| XML           | Implemented   |
| LiteDB        | Implemented   |
| MongoDB       | TBD           |

# References

[1] Farhang, A., Park, G. G., Berti, A., & Aalst, W. Van Der. (2020). OCEL Standard. http://ocel-standard.org/

[2] Berti, A., Ghahfarokhi, A. F., Park, G., & van der Aalst, W. M. P. (2021). A Scalable Database for the Storage of Object-Centric Event Logs. CEUR Workshop Proceedings, 3098, 19–20. https://arxiv.org/abs/2202.05639.