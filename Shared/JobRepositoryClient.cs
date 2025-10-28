using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;
using Shared.Configuration;

namespace Shared
{
    /// <summary>
    /// Convenient client to communicate with JobRepository service.
    /// </summary>
    public partial class JobRepositoryClient : IJobRepositoryClient
    {
        private const string JobApiResource = ServicesConstants.ControllerRoutes.JobsApiResource;
        private const string OdataJobApiResource = ServicesConstants.ControllerRoutes.OdataJobsApiResource;
        private const string AttachmentsApiResource = ServicesConstants.ControllerRoutes.AttachmentsApiResource;

        private readonly ILogger<JobRepositoryClient> logger;
        private readonly JobRepositoryClientConfig config;
        private readonly object authLock = new object();
        private static readonly HttpClient HttpClient = new HttpClient(); // Workaround for file upload issues with RestSharp. RestSharp as of now can't do a stream upload.
        private string authToken = string.Empty;
        private DateTime authTokenExpiry = DateTime.MinValue;

        private string JobServiceUrl => this.config.BaseUrl.TrimEnd('/');
        
        private bool IsAuthEnabled => this.config.AuthClientCredentials != null;
        
        // Add extra 5 seconds before token expires.
        private bool TokenExpired =>
            authTokenExpiry <= DateTime.Now.AddSeconds(5) || string.IsNullOrWhiteSpace(authToken);

        public JobRepositoryClient(ILogger<JobRepositoryClient> logger, IOptions<JobRepositoryClientConfig> config)
        {
            this.logger = logger;
            this.config = config.Value;

            if (string.IsNullOrWhiteSpace(this.config.BaseUrl))
            {
                this.logger.LogError("Missing baseUrl to a repository service!");
                throw new ArgumentException("BaseUrl must be specified in the configuration.");
            }

            if (this.config.AuthClientCredentials == null)
            {
                this.logger.LogDebug("No auth credentials specified. Client will make anonymous requests.");
            }

            this.logger.LogTrace(
                $"Created an instance of {nameof(JobRepositoryClient)} with service url: {config.Value.BaseUrl}");
        }

        private string GetAuthToken()
        {
            this.logger.LogTrace($"Auth enabled: {IsAuthEnabled}");

            if (!IsAuthEnabled)
            {
                return string.Empty;
            }

            this.logger.LogTrace($"Token expired: {TokenExpired}");

            if (!TokenExpired)
            {
                return authToken;
            }

            lock (authLock)
            {
                if (!TokenExpired)
                {
                    return authToken;
                }

                this.logger.LogTrace($"Fetching new auth token from auth server. TokenEndpoint:{this.config.AuthClientCredentials.TokenEndpoint}");

                using (var client = new RestClient())
                {
                    var request = new RestRequest(this.config.AuthClientCredentials.TokenEndpoint)
                        .AddParameter("client_id", this.config.AuthClientCredentials.ClientId)
                        .AddParameter("client_secret", this.config.AuthClientCredentials.ClientSecret)
                        .AddParameter("grant_type", "client_credentials");
                    var tokenResponse = client.Post<AccessTokenResponse>(request);
                    this.authTokenExpiry = DateTime.Now.AddSeconds(tokenResponse.expires_in);
                    this.logger.LogTrace($"New token retrieved. Expires in {authTokenExpiry}");

                    this.authToken = tokenResponse.access_token;

                    this.logger.LogTrace($"Auth token acquired: {this.authToken}");

                    return this.authToken;
                }
            }
        }

        private RestClientOptions Options(string url)
        {
            var options = new RestClientOptions(url);
            if (IsAuthEnabled)
            {
                options.Authenticator = new JwtAuthenticator(GetAuthToken());
            }

            return options;
        }

        private record AccessTokenResponse
        {
            public string access_token { get; set; } = string.Empty;
            public int expires_in { get; set; }
            public string token_type { get; set; } = string.Empty;
        }
    }
}
