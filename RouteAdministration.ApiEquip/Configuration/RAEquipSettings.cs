namespace RouteAdministration.ApiEquip.Configuration
{
    public class RAEquipSettings : IRAEquipSettings
    {
        public string RAEquipCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
