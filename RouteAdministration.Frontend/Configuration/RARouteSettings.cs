namespace RouteAdministration.Frontend.Configuration
{
    public class RARouteSettings : IRARouteSettings
    {
        public string RARouteCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
