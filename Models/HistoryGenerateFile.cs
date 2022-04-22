using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class HistoryGenerateFile
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome do Arquivo")]
        public string FileName { get; set; }
        public string FullPath { get; set; }        

        [Display(Name = "Serviço")]
        public string Service { get; set; }

        [Display(Name = "Cidade")]
        public string City { get; set; }

        [Display(Name = "Data de Geração")]
        public string Date { get; set; }
        public List<string> Headers { get; set; }
        public List<string> Equips { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
