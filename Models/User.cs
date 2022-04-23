using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Usuário")]
        public string Username { get; set; }

        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Tipo")]
        public string Role { get; set; } = "user";
        public string Error { get; set; } = string.Empty;
    }
}
