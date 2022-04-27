using Models;
using MongoDB.Driver;
using RouteAdministration.ApiCity.Configuration;
using System.Collections.Generic;

namespace RouteAdministration.ApiCity.Service
{
    public class ApiCityService
    {
        private readonly IMongoCollection<City> _city;

        public ApiCityService(IRACitySettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _city = database.GetCollection<City>(settings.RACityCollectionName);
        }

        public List<City> Get()
        {
            List<City> city = new();

            city = _city.Find(city => true).ToList();

            return city;
        }

        public City Get(string id)
        {
            City city = new();

            city = _city.Find<City>(city => city.Id == id).FirstOrDefault();

            return city;
        }

        public City GetName(string name)
        {
            City city = new();

            city = _city.Find<City>(city => city.Name == name).FirstOrDefault();

            return city;
        }

        public City Create(City city)
        {
            City existsCity = new();

            existsCity = GetName(city.Name);

            if (existsCity == null)
                _city.InsertOne(city);

            return city;
        }

        public void Update(string id, City cityIn) =>
            _city.ReplaceOne(city => city.Id == id, cityIn);

        public void Remove(string id, City cityIn) =>
            _city.DeleteOne(city => city.Id == cityIn.Id);
    }
}
