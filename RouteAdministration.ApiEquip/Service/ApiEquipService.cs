using MongoDB.Driver;
using RouteAdministration.ApiEquip.Configuration;
using RouteAdministration.ApiEquip.Model;
using System.Collections.Generic;

namespace RouteAdministration.ApiEquip.Service
{
    public class ApiEquipService
    {
        private readonly IMongoCollection<Equip> _team;

        public ApiEquipService(IRAEquipSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _team = database.GetCollection<Equip>(settings.RAEquipCollectionName);
        }

        public List<Equip> Get()
        {
            List<Equip> equip = new();

            equip = _team.Find(equip => true).ToList();

            return equip;
        }
    }
}
