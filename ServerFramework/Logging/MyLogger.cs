using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework.Logging
{
    public class MyLogger
    {
        static MyLogger instance;

        private TraceSource _ts;
        private TraceListener _textListener;

        
        private MyLogger()
        {
            //Initialization of the tracesource and sourceswitch
            _ts = new TraceSource("Server Logger", SourceLevels.All);
            _ts.Switch = new SourceSwitch("log", SourceLevels.All.ToString());

            //Initialization of a TextWriterListener as default
            AddTextWriterListener("mylog.txt");
            AddFilterToListener(ListenerType.TextWriter, SourceLevels.All);
        }

        public static MyLogger Instance()
        {
            if (instance == null)
            {
                instance = new MyLogger();
            }
        
            return instance;
        }

        // Wrapper methods for different log levels
        public void LogInfo(string message)
        {
            _ts.TraceEvent(TraceEventType.Information, 5, $"{DateTime.Now}: {message}");
        }

        public void LogWarning(string message)
        {
            _ts.TraceEvent(TraceEventType.Warning, 10, $"{DateTime.Now}: {message}");
        }

        public void LogError(string message)
        {
            _ts.TraceEvent(TraceEventType.Error, 15, $"{DateTime.Now}: {message}");
        }

        public void LogError(string message, Exception ex)
        {
            _ts.TraceEvent(TraceEventType.Error, 15, $"{DateTime.Now}: {message} - Exception: {ex.Message}");
        }

        public void LogCritical(string message)
        {
            _ts.TraceEvent(TraceEventType.Critical, 20, $"{DateTime.Now}: {message}");
        }

        public void LogVerbose(string message)
        {
            _ts.TraceEvent(TraceEventType.Verbose, 1, $"{DateTime.Now}: {message}");
        }

        // Server-specific logging methods
        public void LogServerStart(string serverName, int port)
        {
            LogInfo($"{serverName} started at port: {port}");
        }

        public void LogClientConnected(string remoteEndPoint)
        {
            LogInfo($"Client connected: remote (ip, port) = ({remoteEndPoint})");
        }

        public void LogStopServerStart(int stopPort)
        {
            LogInfo($"Stop server started at port {stopPort}");
        }

        // Flush method
        public void Flush()
        {
            _ts.Flush();
        }

        // Methods to add TraceListeners
        public void AddConsoleListener()
        {
            _ts.Listeners.Add(new ConsoleTraceListener());
        }

        public void AddXmlListener(string logFilePath)
        {
            _ts.Listeners.Add(new XmlWriterTraceListener(logFilePath));
        }

        public void AddTextWriterListener(string logFilePath)
        {
            _ts.Listeners.Add(new TextWriterTraceListener(logFilePath));
        }

        public void AddEventLogListener(string logName = "Application")
        {
            _ts.Listeners.Add(new EventLogTraceListener(logName));
        }

        // Method to add Trace filters
        public void AddFilterToListener(ListenerType listenerType, SourceLevels sourceLevel)
        {
            foreach (TraceListener listener in _ts.Listeners)
            {
                bool shouldAddFilter = listenerType switch
                {
                    ListenerType.Console => listener is ConsoleTraceListener,
                    ListenerType.Xml => listener is XmlWriterTraceListener,
                    ListenerType.TextWriter => listener is TextWriterTraceListener,
                    ListenerType.EventLog => listener is EventLogTraceListener,
                    ListenerType.All => true,
                    _ => false
                };

                if (shouldAddFilter)
                {
                    listener.Filter = new EventTypeFilter(sourceLevel);
                }
            }
        }
    }
}
