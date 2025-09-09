using TCPFrameworkDemo.Server;

namespace TCPFrameworkDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string configPath = Environment.GetEnvironmentVariable("AbstractServerConfig");

            MyServer server = new(configPath + "serverconfig.xml");

            server.Start();
        }
    }
}
