namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World from Server!");

            var server = new WebSocketServer();
            server.StartAsync().Wait();
        }
    }
}
