namespace RouteAdministration.ApiTeam.Configuration
{
    public interface IRATeamSettings
    {
        public string RATeamCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
