namespace Shared.Configuration
{
    public class AuthOptionsConfig
    {
        /// <summary>
        /// Url to a auth server including realm.
        /// Example: http://localhost:8080/realms/yourrealm
        /// </summary>
        public string Authority { get; set; }

        // Client ID of the a requesting application registered in Keycloak
        public string ClientId { get; set; }

        public bool RequireHttpsMetadata { get; set; } = false; // dev only
    }
}
