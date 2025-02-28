using System.Collections.Concurrent;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly  Serilog.ILogger _logger;
        private readonly RequestCreator _requestCreator;
        private readonly RequestProcessor _requestProcessor;
        private bool _isRunning = true;

        public Worker(Serilog.ILogger logger, RequestCreator requestCreator, RequestProcessor requestProcessor)
        {
            _logger = logger;
            _requestCreator = requestCreator;
            _requestProcessor = requestProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var requests = new ConcurrentQueue<Request>();
                var tasks = new List<Task>();
                tasks.Add(_requestCreator.Start(requests, stoppingToken));
                tasks.Add(_requestProcessor.Start(requests, stoppingToken));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _isRunning = false;
            }

            while (!stoppingToken.IsCancellationRequested && _isRunning)
            {

                _logger.Information("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
