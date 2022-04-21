namespace RouteAdministration.Frontend.Configuration
{
    public interface IRARouteSettings
    {
        public string RARouteCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
