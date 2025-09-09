using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ServerFramework.TCPServer
{
    /// <summary>
    /// Template class for a TCP server.
    /// </summary>
    public abstract class AbstractTCPServer
    {
        private readonly int PORT;
        private readonly int STOPPORT;
        private readonly string? _serverName;

        private bool _running = true;

        protected TraceSource _ts;
        private TraceListener _consoleListener;
        private TraceListener _textListener;
        private TraceListener _xmlListener;
        private TraceListener _eventLogListener;

        /// <param name="configFile">A configuration file containing information regarding the ports and servername</param>
        public AbstractTCPServer(string configFile) 
        {
            _ts = new TraceSource("Server Logger", SourceLevels.All);
            _ts.Switch = new SourceSwitch("log", SourceLevels.All.ToString());

            _consoleListener = new ConsoleTraceListener();
            _textListener = new TextWriterTraceListener("textlog.txt");
            _textListener.Filter = new EventTypeFilter(SourceLevels.Warning);
            _xmlListener = new XmlWriterTraceListener("xmllog.txt");
            _eventLogListener = new EventLogTraceListener("application");

            _ts.Listeners.Add(_consoleListener);
            _ts.Listeners.Add(_textListener);
            _ts.Listeners.Add(_xmlListener);
            _ts.Listeners.Add(_eventLogListener);

            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(configFile);

            XmlNode serverPortNode = configDoc.DocumentElement.SelectSingleNode("ServerPort");
            if (serverPortNode != null)
            { 
                PORT = Convert.ToInt32(serverPortNode.InnerText);
            }

            XmlNode shutdownServerPortNode = configDoc.DocumentElement.SelectSingleNode("ShutdownPort");
            if (shutdownServerPortNode != null)
            {
                STOPPORT = Convert.ToInt32(shutdownServerPortNode.InnerText);
            }

            XmlNode serverNameNode = configDoc.DocumentElement.SelectSingleNode("ServerName");
            if (serverNameNode != null)
            { 
                _serverName = serverNameNode.InnerText;
            }
        }

        /// <summary>
        /// Template method for the class which sets up a TCP listener and waits for a client to connect. <br/>
        /// When a client connects a new task will be run to handle the and the specific work of the <br/> 
        /// derived classes will be executed.
        /// </summary>
        public void Start()
        {
            TcpListener listener = new(IPAddress.Any, PORT);
            listener.Start();

            _ts.TraceEvent(TraceEventType.Information, 5, $"{_serverName}");
            _ts.TraceEvent(TraceEventType.Information, 5, $"{DateTime.Now}: {_serverName} started at port: {PORT}");

            Task.Run(StopServer);

            List<Task> tasks = new List<Task>();
            while (_running)
            {
                _ts.Flush();
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _ts.TraceEvent(TraceEventType.Information, 5, $"{DateTime.Now}: Client connected:");
                    _ts.TraceEvent(TraceEventType.Information, 5, $"remote (ip, port) = ({client.Client.RemoteEndPoint})");

                    tasks.Add(
                    Task.Run(() =>
                    {
                        TcpClient tmpClient = client;
                        DoOneClient(client);
                    })
                    );
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// Concrete method that defines the specific work to each derived TCP server.
        /// </summary>
        /// <param name="reader">StreamReader object passed from the client</param>
        /// <param name="writer">StreamWriter object passed from the client</param>
        protected abstract void TcpServerWork(StreamReader reader, StreamWriter writer);

        private void DoOneClient(TcpClient sock)
        {
            using StreamReader sr = new(sock.GetStream());
            using StreamWriter sw = new(sock.GetStream());

            TcpServerWork(sr, sw);

            sock?.Close();
        }

        protected virtual void StopServer()
        {
            TcpListener listener = new(IPAddress.Any, STOPPORT);
            listener.Start();
            _ts.TraceEvent(TraceEventType.Information, 5, $"Stop server started at port {STOPPORT}");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                StreamReader sr = new(client.GetStream());
                string? command = sr.ReadLine();

                if (!string.IsNullOrEmpty(command) && command == "stop")
                {
                    _running = false;
                    return;
                }
            }
        }
    }
}
