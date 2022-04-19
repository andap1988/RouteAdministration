using MongoDB.Bson.Serialization.Attributes;

namespace RouteAdministration.ApiEquip.Model
{
    public class Equip
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }
}
