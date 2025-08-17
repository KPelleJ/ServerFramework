using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework.TCPServer
{
    /// <summary>
    /// Template class for a TCP server.
    /// </summary>
    public abstract class AbstractTCPServer
    {
        private readonly int PORT;
        protected readonly string? _serverName;

        
        /// <param name="port">Object must come with an associated port number</param>
        /// <param name="name">Server name is optional</param>
        public AbstractTCPServer(int port, string? name) 
        { 
            PORT = port;
            _serverName = name;
        }

        /// <summary>
        /// Template method for the class which sets up a TCP listener and waits for a client to connect. <br/>
        /// When a client connects a new task will be run to handle the and the specific work of the <br/> 
        /// derived classes will be executed.
        /// </summary>
        public void Start()
        {
            TcpListener listener = new(System.Net.IPAddress.Any, PORT);
            listener.Start();
            Console.WriteLine($"{DateTime.Now}: {_serverName} started at port: {PORT}");


            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine($"{DateTime.Now}: Client connected:");
                Console.WriteLine($"remote (ip, port) = ({client.Client.RemoteEndPoint})");

                Task.Run(() =>
                {
                    TcpClient tmpClient = client;
                    DoOneClient(client);
                });
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
        }


    }
}
