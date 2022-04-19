namespace RouteAdministration.ApiCity.Configuration
{
    public interface IRACitySettings
    {
        public string RACityCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
