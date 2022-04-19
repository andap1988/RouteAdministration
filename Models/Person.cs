using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Models
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
