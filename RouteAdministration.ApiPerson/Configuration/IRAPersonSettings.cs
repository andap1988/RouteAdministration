namespace RouteAdministration.ApiPerson.Configuration
{
    public interface IRAPersonSettings
    {
        public string RAPersonCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
