namespace RouteAdministration.ApiTeam.Configuration
{
    public class RATeamSettings : IRATeamSettings
    {
        public string RATeamCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
