using ServerFramework.TCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPFrameworkDemo.Server
{
    public class MyServer : AbstractTCPServer
    {
        public MyServer(int port) : base(port, "TCP Echo Server")
        {
        }

        protected override void TcpServerWork(StreamReader reader, StreamWriter writer)
        {
            writer.AutoFlush = true;
            Console.WriteLine($"Handling new client");

            string? msg = reader.ReadLine();

            writer.WriteLine(msg?.ToUpper() ?? "Your message was empty");
        }
    }
}
