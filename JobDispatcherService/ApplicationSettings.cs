using Shared.Configuration;

namespace JobDispatcherService
{
    public class ApplicationSettings 
    {
        public JobRepositoryClientConfig JobRepositoryConfig { get; set; }

        public RabbitConfig RabbitConfig { get; set; }
    }
}
