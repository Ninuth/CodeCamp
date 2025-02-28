using System.Collections.Concurrent;

using ILogger = Serilog.ILogger;

namespace WorkerService1
{
    /// <summary>
    /// Creates requests and adds to queue.
    /// </summary>
    public class RequestCreator
    {
        private readonly ILogger _logger;
        private readonly Config _config;

        public RequestCreator(Serilog.ILogger logger, Config config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task Start(ConcurrentQueue<Request> requests, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _config.Threads; i++)
            {
                await CreateRequest(requests, i, cancellationToken);
            }
        }

        private async Task CreateRequest(ConcurrentQueue<Request> requests, int threadId, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var request = new Request()
                {
                    ReceivedDate = DateTime.Now,
                    Message = $"test {threadId}"
                };

                requests.Enqueue(request);
                await Task.Delay(5000);
            }
        }
    }
}
