namespace Shared.Configuration
{
    public class AuthOptionsConfig : AuthServerConfig
    {
        /// <summary>
        /// Url to a auth server including realm. Built from Authority + Realm.
        /// Example: http://localhost:8080/realms/yourrealm
        /// </summary>
        public string RealmAuthority => $"{Authority?.TrimEnd('/')}/realms/{Realm}";

        // Collection of audiences (Client IDs of the a requesting application registered in Keycloak)
        /// that are allowed to access the resources protected by this service.
        public string[] Audiences { get; set; }

        public bool RequireHttpsMetadata { get; set; } = false; // dev only
    }
}
