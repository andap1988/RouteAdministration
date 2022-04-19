namespace RouteAdministration.ApiCity.Configuration
{
    public class RACitySettings : IRACitySettings
    {
        public string RACityCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
