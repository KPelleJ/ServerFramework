using ServerFramework.TCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TCPFrameworkDemo.Server
{
    public class MyServer : AbstractTCPServer
    {

        public MyServer(string configFile): base(configFile)
        {

        }

        protected override void TcpServerWork(StreamReader reader, StreamWriter writer)
        {
            writer.AutoFlush = true;
            _ts.TraceEvent(TraceEventType.Warning, 4, $"{DateTime.Now}: Handling new client");
            
            string? msg = reader.ReadLine();

            writer.WriteLine(msg?.ToUpper() ?? "Your message was empty");
        }
    }
}
