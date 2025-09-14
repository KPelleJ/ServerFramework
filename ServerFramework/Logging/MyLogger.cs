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

        public TraceSource Ts;
        private TraceListener _consoleListener;
        private TraceListener _textListener;
        private TraceListener _xmlListener;
        private TraceListener _eventLogListener;

        
        private MyLogger()
        {
            //Initialization of the tracesource and sourceswitch
            Ts = new TraceSource("Server Logger", SourceLevels.All);
            Ts.Switch = new SourceSwitch("log", SourceLevels.All.ToString());

            //Initialization of the tracelisteners
            _consoleListener = new ConsoleTraceListener();
            _textListener = new TextWriterTraceListener("textlog.txt");
            _xmlListener = new XmlWriterTraceListener("xmllog.txt");
            _eventLogListener = new EventLogTraceListener("application");

            //Setup of filters for the listeners
            _xmlListener.Filter = new EventTypeFilter(SourceLevels.Warning);

            //Adding the tracelisteners to the tracesource
            Ts.Listeners.Add(_consoleListener);
            Ts.Listeners.Add(_textListener);
            Ts.Listeners.Add(_xmlListener);
            Ts.Listeners.Add(_eventLogListener);
        }

        public static MyLogger Instance()
        {
            if (instance == null)
            {
                instance = new MyLogger();
            }
        
            return instance;
        }
    }
}
