namespace Shared.Configuration
{
    public class AuthClientCredentials : AuthServerConfig
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        /// <summary>
        /// Full url for obtaining tokens.
        /// Example:http://localhost:8080/realms/JobQueueSystemRealm/protocol/openid-connect/token
        /// </summary>
        public string TokenEndpoint => $"{Authority?.TrimEnd('/')}/realms/{Realm}/protocol/openid-connect/token";
    }
}
