namespace Shared.Configuration
{
    public abstract class AuthServerConfig
    {
        /// <summary>
        /// Url to a auth server.
        /// Example: http://localhost:8080
        /// </summary>
        public string? Authority { get; set; }

        ///realms/yourrealm
        /// <summary>
        /// Realm name.
        /// </summary>
        public string? Realm { get; set; }
    }
}
