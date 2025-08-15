using TCPFrameworkDemo.Server;

namespace TCPFrameworkDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            MyServer server = new(7);

            server.Start();
        }
    }
}
