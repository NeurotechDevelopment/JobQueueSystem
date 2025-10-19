using Shared.Configuration;

namespace JobRepositoryService
{
    public class ApplicationSettings
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        /// <summary>
        /// Seconds before a temporary file link expires.
        /// </summary>
        public int TempFileLinkExpirationInSeconds { get; set; } = 300; // Default to 5 minutes

        public AuthOptionsConfig AuthOptions { get; set; }
    }
}
