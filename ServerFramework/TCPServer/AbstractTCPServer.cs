using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework.TCPServer
{
    public abstract class AbstractTCPServer
    {
        private readonly int PORT;
        protected readonly string _serverName;

        public AbstractTCPServer(int port, string name) 
        { 
            PORT = port;
            _serverName = name;
        }

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

        protected abstract void TcpServerWork(StreamReader reader, StreamWriter writer);

        private void DoOneClient(TcpClient sock)
        {
            using StreamReader sr = new(sock.GetStream());
            using StreamWriter sw = new(sock.GetStream());

            TcpServerWork(sr, sw);
        }


    }
}
