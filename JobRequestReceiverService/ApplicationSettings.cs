using Shared.Configuration;

namespace JobRequestReceiverService
{
    internal class ApplicationSettings
    {
        public RabbitConfig RabbitConfig { get; set; }

        public JobRepositoryClientConfig JobRepositoryClientConfig { get; set; }
    }
}
