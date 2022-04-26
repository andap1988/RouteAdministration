using Models;
using MongoDB.Driver;
using RouteAdministration.Frontend.Configuration;
using RouteAdministration.Frontend.Models;
using System.Collections.Generic;
using System.IO;

namespace RouteAdministration.Frontend.Service
{
    public class RARouteService
    {
        private readonly IMongoCollection<HistoryGenerateFile> _historyGenerateFile;

        public RARouteService(IRARouteSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _historyGenerateFile = database.GetCollection<HistoryGenerateFile>(settings.RARouteCollectionName);
        }

        public List<HistoryGenerateFile> Get()
        {
            List<HistoryGenerateFile> hgFile = new();

            hgFile = _historyGenerateFile.Find(hgFile => true).ToList();

            return hgFile;
        }

        public HistoryGenerateFile Get(string id)
        {
            HistoryGenerateFile hgFile = new();

            hgFile = _historyGenerateFile.Find(hgFile => hgFile.Id == id).FirstOrDefault();

            return hgFile;
        }
        public HistoryGenerateFile GetFileByName(string fileName)
        {
            HistoryGenerateFile hgFile = new();

            hgFile = _historyGenerateFile.Find(hgFile => hgFile.FileName == fileName).FirstOrDefault();

            return hgFile;
        }

        public void Create(HistoryGenerateFile hgFile)
        {
            _historyGenerateFile.InsertOne(hgFile);
        }

        public async void Delete(string id)
        {
            _historyGenerateFile.DeleteOne(equip => equip.Id == id);
        }
    }
}
