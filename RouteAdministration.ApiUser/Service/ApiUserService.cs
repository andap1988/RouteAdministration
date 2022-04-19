using Models;
using MongoDB.Driver;
using RouteAdministration.ApiUser.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteAdministration.ApiUser.Service
{
    public class ApiUserService
    {
        private readonly IMongoCollection<User> _user;

        public ApiUserService(IRAUserSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _user = database.GetCollection<User>(settings.RAUserCollectionName);
        }

        public List<User> Get()
        {
            List<User> users = new();

            users = _user.Find(user => true).ToList();

            return users;
        }

        public User Get(string id)
        {
            User user = new();

            user = _user.Find<User>(user => user.Id == id).FirstOrDefault();

            return user;
        }

        public User GetUserByUsername(string username)
        {
            User user = new();

            user = _user.Find<User>(user => user.Username == username).FirstOrDefault();

            return user;
        }

        public async Task<User> Create(User user)
        {
            var verifyUser = GetUserByUsername(user.Username);

            if (verifyUser != null)
            {
                user.Error = "yesUser";

                return user;
            }

            _user.InsertOne(user);

            return user;
        }

        public void Update(string id, User userIn)
        {
            _user.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(string id, User userIn)
        {
            _user.DeleteOne(user => user.Id == userIn.Id);
        }
    }
}
