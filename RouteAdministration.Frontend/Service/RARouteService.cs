using MongoDB.Driver;
using RouteAdministration.Frontend.Configuration;
using RouteAdministration.Frontend.Models;
using System.Collections.Generic;

namespace RouteAdministration.Frontend.Service
{
    public class RARouteService
    {
        private readonly IMongoCollection<Table> _table;

        public RARouteService(RARouteSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _table = database.GetCollection<Table>(settings.RARouteCollectionName);
        }

        public List<Table> Get()
        {
            List<Table> city = new();

            city = _table.Find(city => true).ToList();

            return city;
        }
        public void Create(Table table)
        {
            _table.InsertOne(table);

        }
    }
}
