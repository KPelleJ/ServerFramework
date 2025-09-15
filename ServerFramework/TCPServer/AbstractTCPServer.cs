using ServerFramework.Logging;
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

        protected MyLogger _logger = MyLogger.Instance();

        /// <param name="configFile">A configuration file containing information regarding the ports and servername</param>
        public AbstractTCPServer(string configFile) 
        {
            // Setting up server configuration from configfile
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

            // Setting up logging from MyLogger class
            _logger.AddConsoleListener();
            _logger.AddFilterToListener(ListenerType.Console, SourceLevels.All);
            _logger.AddXmlListener("mylog.xml");
            _logger.AddFilterToListener(ListenerType.Xml, SourceLevels.All);
            _logger.AddEventLogListener();
            _logger.AddFilterToListener(ListenerType.EventLog, SourceLevels.Warning);
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

            _logger.LogInfo(_serverName ?? "Unkown Server");
            _logger.LogServerStart(_serverName ?? "Unkown Server", PORT);

            Task.Run(StopServer);

            List<Task> tasks = new List<Task>();
            while (_running)
            {
                _logger.Flush();
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _logger.LogClientConnected(client.Client.RemoteEndPoint?.ToString() ?? "Uknown");


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

            try
            {
                TcpServerWork(sr, sw);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error handlign client", ex);
            }
            

            sock?.Close();
        }

        protected virtual void StopServer()
        {
            TcpListener listener = new(IPAddress.Any, STOPPORT);
            listener.Start();
            _logger.LogStopServerStart(STOPPORT);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                StreamReader sr = new(client.GetStream());
                string? command = sr.ReadLine();

                if (!string.IsNullOrEmpty(command) && command == "stop")
                {
                    _logger.LogInfo("Stop command received, shutting down server");
                    _running = false;
                    return;
                }
            }
        }
    }
}
