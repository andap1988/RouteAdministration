using MongoDB.Driver;
using RouteAdministration.ApiTeam.Configuration;
using RouteAdministration.ApiTeam.Model;
using System.Collections.Generic;

namespace RouteAdministration.ApiTeam.Service
{
    public class ApiTeamService
    {
        /*private readonly IMongoCollection<Team> _team;

        public ApiTeamService(IRATeamSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _team = database.GetCollection<Team>(settings.RATeamCollectionName);
        }

        public List<Team> Get()
        {
            List<Team> team = new();

            team = _team.Find(team => true).ToList();

            return team;
        }

        public Team Get(string id)
        {
            Team team = new();

            team = _team.Find<Team>(team => team.Id == id).FirstOrDefault();

            return team;
        }

        public Team Create(Team team)
        {
            _team.InsertOne(team);

            return team;
        }

        public void Update(string id, Team teamIn)
        {
            _team.ReplaceOne(team => team.Id == id, teamIn);
        }

        public void Remove(string id, Team teamIn)
        {
            _team.DeleteOne(team => team.Id == teamIn.Id);
        }*/
    }
}
