using Shared.Configuration;

namespace JobProducerService.Configuration
{
    public class ApplicationSettings
    {
        public int? MaxAttachmentSizeInBytes { get; set; }

        public JobRepositoryClientConfig JobRepositoryClientConfig { get; set; }

        public RabbitConfig RabbitConfig { get; set; }

        public AuthOptionsConfig AuthOptions { get; set; }
    }
}
