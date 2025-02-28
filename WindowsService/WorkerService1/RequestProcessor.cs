using System.Collections.Concurrent;

using ILogger = Serilog.ILogger;

namespace WorkerService1
{
    /// <summary>
    /// Processes requests from queue.
    /// </summary>
    public class RequestProcessor
    {
        private readonly ILogger _logger;
        private readonly Config _config;

        public RequestProcessor(Serilog.ILogger logger, Config config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task Start(ConcurrentQueue<Request> requests, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _config.Threads; i++)
            {
                await ProcessRequest(requests, i, cancellationToken);
            }
        }

        private async Task ProcessRequest(ConcurrentQueue<Request> requests, int threadId, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!requests.TryDequeue(out Request? request))
                {
                    await Task.Delay(5000);
                    continue;
                }

                _logger.Information($"Processed request : {request.Message} received at : {request.ReceivedDate}.");
                await Task.Delay(5000);
            }
        }
    }
}
