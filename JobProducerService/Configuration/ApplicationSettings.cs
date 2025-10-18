using Shared.Configuration;

namespace JobProducerService.Configuration
{
    public class ApplicationSettings
    {
        public JobRepositoryClientConfig JobRepositoryClientConfig { get; set; }

        public RabbitConfig RabbitConfig { get; set; }

        public AuthOptionsConfig AuthOptions { get; set; }
    }
}
