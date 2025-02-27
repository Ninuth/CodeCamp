using System.Net.WebSockets;
using System.Text;

namespace Client
{
    internal class WebSocketClient
    {
        private readonly ClientWebSocket myClient;
        private bool myIsCancelled = false;

        public WebSocketClient()
        {
            myClient = new ClientWebSocket();
        }

        /// <summary>
        /// Establishes connection with server
        /// </summary>
        public async Task ConnectAsync()
        {
            var serverUri = new Uri("ws://localhost:443");
            string token = "testToken";
            //token can be appended with current date for added security
            //encrypt token using a shared key

            myClient.Options.SetRequestHeader("Authorization", token);
            myClient.Options.SetRequestHeader("App-Name", "TestApp");
            myClient.Options.SetRequestHeader("App-Version", "1.0.0");
            await myClient.ConnectAsync(serverUri, CancellationToken.None);
        }

        /// <summary>
        /// Sends and receives messages to/from server.
        /// </summary>
        public async Task StartCommunication()
        {
            var tasks = new List<Task>();
            tasks.Add(SendMessagesAsync());
            tasks.Add(ReceiveMessageAsync());

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends a message to server and receives response back.
        /// </summary>
        public async Task StartSingleCommunication()
        {
            await SendMessageAsync("hello from client");
            await ReceiveMessageAsync();
        }

        private async Task SendMessagesAsync()
        {
            while (myClient.State == WebSocketState.Open)
            {
                await SendMessageAsync($"Hello from client at {DateTime.Now}");
                await Task.Delay(3000);
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await myClient.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"Sent: {message}");
        }

        private async Task ReceiveMessageAsync()
        {

            var buffer = new byte[1024 * 4];
            while (myClient.State == WebSocketState.Open)
            {
                var result = await myClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Server closed the connection.");
                    await myClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                else
                {
                    Console.WriteLine($"Received: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                }
            }
        }

        public async Task CloseAsync()
        {
            await myClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            Console.WriteLine("Client closed connection.");
        }
    }
}
