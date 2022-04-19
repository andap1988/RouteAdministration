using Models;
using MongoDB.Driver;
using RouteAdministration.ApiEquip.Configuration;
using System.Collections.Generic;

namespace RouteAdministration.ApiEquip.Service
{
    public class ApiEquipService
    {
        private readonly IMongoCollection<Equip> _equip;

        public ApiEquipService(IRAEquipSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _equip = database.GetCollection<Equip>(settings.RAEquipCollectionName);
        }

        public List<Equip> Get()
        {
            List<Equip> equip = new();

            equip = _equip.Find(equip => true).ToList();

            return equip;
        }

        public Equip Get(string id)
        {
            Equip equip = new();

            equip = _equip.Find<Equip>(equip => equip.Id == id).FirstOrDefault();

            return equip;
        }

        public Equip Create(Equip equip)
        {
            _equip.InsertOne(equip);

            return equip;
        }

        public void Update(string id, Equip equipIn)
        {
            _equip.ReplaceOne(equip => equip.Id == id, equipIn);
        }

        public void Remove(string id, Equip equipIn)
        {
            _equip.DeleteOne(equip => equip.Id == equipIn.Id);
        }
    }
}
