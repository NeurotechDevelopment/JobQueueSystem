namespace JobHandlers
{
    public class DummyHandlerWorker : BackgroundService
    {
        private readonly ILogger<DummyHandlerWorker> logger;

        public DummyHandlerWorker(ILogger<DummyHandlerWorker> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("DummyHandlerWorker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
