namespace RouteAdministration.ApiUser.Configuration
{
    public class RAUserSettings : IRAUserSettings
    {
        public string RAUserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
