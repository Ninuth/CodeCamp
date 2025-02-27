namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World from Client!");

            StartClient().Wait();
        }

        static async Task StartClient()
        {
            var client = new WebSocketClient();
            await client.ConnectAsync();
            await client.StartCommunication();
            //await client.StartSingleCommunication();

            await client.CloseAsync();
        }
    }
}
