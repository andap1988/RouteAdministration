using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Equip
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Cidade")]
        public string City { get; set; }

        [Display(Name = "Pessoas da Equipe")]
        public List<Person> People { get; set; } = new();
        public bool Status { get; set; } = true;
        public string Error { get; set; } = String.Empty;
    }
}
