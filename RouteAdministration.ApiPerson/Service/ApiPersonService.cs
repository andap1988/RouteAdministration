using Models;
using MongoDB.Driver;
using RouteAdministration.ApiPerson.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.ApiPerson.Service
{
    public class ApiPersonService
    {
        private readonly IMongoCollection<Person> _person;

        public ApiPersonService(IRAPersonSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _person = database.GetCollection<Person>(settings.RAPersonCollectionName);
        }

        public List<Person> Get()
        {
            List<Person> people = new();

            people = _person.Find(person => true).ToList();

            return people;
        }

        public Person Get(string id)
        {
            Person person = new();

            person = _person.Find<Person>(person => person.Id == id).FirstOrDefault();

            return person;
        }

        public Person GetByName(string name)
        {
            Person person = new();

            person = _person.Find<Person>(person => person.Name == name).FirstOrDefault();

            return person;
        }

        public Person Create(Person person)
        {
            _person.InsertOne(person);

            return person;
        }

        public void Update(string id, Person personIn)
        {
            _person.ReplaceOne(person => person.Id == id, personIn);
        }

        public void Remove(string id, Person personIn)
        {
            _person.DeleteOne(person => person.Id == personIn.Id);
        }
    }
}
