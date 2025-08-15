using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerFramework.UDPReceiver
{
    public abstract class AbstractUDPReceiver
    {
        private readonly int _port;

        protected AbstractUDPReceiver(int port)
        {
            _port = port;
        }

        public void Receive()
        {
            UdpClient receivingUdpClient = new UdpClient(_port);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                Task.Run(() =>
                {
                    UDPReceiverWork(receivingUdpClient, RemoteIpEndPoint);
                });
            }
        }

        protected abstract void UDPReceiverWork(UdpClient receiver, IPEndPoint sender);
    }
}
