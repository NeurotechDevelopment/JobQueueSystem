namespace Shared.Configuration
{
    public class JobRepositoryClientConfig
    {
        /// <summary>
        /// Url to a Job Repository Service.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Optional client credentials to authenticate to the Job Repository Service.
        /// Contains credentials for retrieving token from Keycloak.
        /// When null, no authentication is used.
        /// </summary>
        public AuthClientCredentials? AuthClientCredentials { get; set; }
    }
}
