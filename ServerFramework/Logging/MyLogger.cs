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
        private TraceListener _consoleListener;
        private TraceListener _textListener;
        private TraceListener _xmlListener;
        private TraceListener _eventLogListener;

        
        private MyLogger()
        {
            //Initialization of the tracesource and sourceswitch
            _ts = new TraceSource("Server Logger", SourceLevels.All);
            _ts.Switch = new SourceSwitch("log", SourceLevels.All.ToString());

            //Initialization of the tracelisteners
            _consoleListener = new ConsoleTraceListener();
            _textListener = new TextWriterTraceListener("textlog.txt");
            _xmlListener = new XmlWriterTraceListener("xmllog.txt");
            _eventLogListener = new EventLogTraceListener("application");

            //Setup of filters for the listeners
            _xmlListener.Filter = new EventTypeFilter(SourceLevels.Warning);

            //Adding the tracelisteners to the tracesource
            _ts.Listeners.Add(_consoleListener);
            _ts.Listeners.Add(_textListener);
            _ts.Listeners.Add(_xmlListener);
            _ts.Listeners.Add(_eventLogListener);
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
    }
}
