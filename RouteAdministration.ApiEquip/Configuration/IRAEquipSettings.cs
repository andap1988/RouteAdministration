namespace RouteAdministration.ApiEquip.Configuration
{
    public interface IRAEquipSettings
    {
        public string RAEquipCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
