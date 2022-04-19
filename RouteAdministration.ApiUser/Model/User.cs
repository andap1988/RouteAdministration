using MongoDB.Bson.Serialization.Attributes;

namespace RouteAdministration.ApiUser.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
