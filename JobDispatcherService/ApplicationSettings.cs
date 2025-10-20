using Shared.Configuration;

namespace JobDispatcherService
{
    public class ApplicationSettings 
    {
        public JobRepositoryClientConfig JobRepositoryClientConfig { get; set; }

        public RabbitConfig RabbitConfig { get; set; }
    }
}
