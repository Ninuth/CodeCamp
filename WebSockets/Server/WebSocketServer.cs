using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Server
{
    internal class WebSocketServer
    {
        private readonly HttpListener myListener;
        private readonly ConnectionManager myConnectionManager;

        public WebSocketServer()
        {
            myListener = new HttpListener();
            myListener.Prefixes.Add("http://localhost:443/");
            myConnectionManager = new ConnectionManager();
        }

        /// <summary>
        /// Starts the WebSocket server and listens for incoming connections. 
        /// When a WebSocket request is received, it processes the request
        /// </summary>
        public async Task StartAsync()
        {
            myListener.Start();
            Console.WriteLine("WebSocket server started.");

            while (true)
            {
                var context = await myListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessRequestAsync(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async void ProcessRequestAsync(HttpListenerContext context)
        {
            bool isValidationSuccess = Validate(context);
            if (isValidationSuccess)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                var clientConnection = new ClientConnection(webSocketContext.WebSocket);
                myConnectionManager.AddConnection(clientConnection);

                await clientConnection.ListenAsync();
            }
        }

        private bool Validate(HttpListenerContext context)
        {
            //Token based authentication
            var token = context.Request.Headers["Authorization"];
            if (!IsTokenValid(token))
            {
                context.Response.StatusCode = 401;
                context.Response.Close();
                return false;
            }

            //Client Certificate based authentication
            //var clientCertificate = context.Request.GetClientCertificate();
            //if (clientCertificate == null || IsClientCertificateValid(clientCertificate))
            //{
            //    context.Response.StatusCode = 403;
            //    context.Response.Close();
            //    continue;
            //}


            string appName = context.Request.Headers["App-Name"];
            string appVersion = context.Request.Headers["App-Version"];
            if (string.Equals(appName, "TestApp") && string.Equals(appVersion, "1.0.0"))
            {
                return true;
            }

            return false;
        }

        private bool IsClientCertificateValid(X509Certificate clientCertificate)
        {
            //var hash = clientCertificate.GetCertHash().Select(b => b.ToString("X2"));
            //compare thumbprint
            return true;
        }

        private bool IsTokenValid(string? token)
        {
            string testToken = "testToken";
            if (token != null && token == testToken)
            {
                return true;
            }

            return false;
        }
    }
}
