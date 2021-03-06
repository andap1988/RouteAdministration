using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RouteAdministration.ApiTeam.Model
{
    public class Team
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }
        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string City { get; set; }
        
        /*[Display(Name = "Lista de Pessoas")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public List<Person> Person { get; set; }*/
    }
}
