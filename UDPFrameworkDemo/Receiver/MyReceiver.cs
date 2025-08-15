using ServerFramework.UDPReceiver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPFrameworkDemo.Receiver
{
    public class MyReceiver : AbstractUDPReceiver
    {
        public MyReceiver(int port) : base(port)
        {
        }

        protected override void UDPReceiverWork(UdpClient receiver, IPEndPoint sender)
        {
            try
            {

                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = receiver.Receive(ref sender);

                string receivedMsg = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine("This is the message you received " +
                                          receivedMsg.ToString());
                Console.WriteLine("This message was sent from " +
                                            sender.Address.ToString() +
                                            " on their port number " +
                                            sender.Port.ToString());
                
                string ackMsg = $"Message Received: {receivedMsg.ToUpper()}";
                byte[] returnData = Encoding.ASCII.GetBytes(ackMsg);

                receiver.Send(returnData, sender);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
