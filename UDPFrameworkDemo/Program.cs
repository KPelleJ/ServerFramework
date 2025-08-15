using UDPFrameworkDemo.Receiver;

namespace UDPFrameworkDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyReceiver udpRec = new(9999);

            udpRec.Receive();
        }
    }
}
