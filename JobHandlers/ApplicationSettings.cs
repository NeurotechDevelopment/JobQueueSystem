using Shared.Configuration;

namespace JobHandlers
{
    public class ApplicationSettings
    {
        public RabbitConfig RabbitConfig { get; set; }

        public JobRepositoryClientConfig JobRepositoryClientConfig { get; set; }
    }
}
