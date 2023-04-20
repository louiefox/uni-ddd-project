using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DDDProject.Data
{
    public class SocietyDataService
    {
        public Task<List<SocietyData>> RequestSocietiesList()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");
            var collection = database.GetCollection<BsonDocument>("Societies");

            var documents = collection.Find(new BsonDocument()).ToList();
            
            List<SocietyData> societiesList = new();
            foreach(BsonDocument doc in documents)
            {
                societiesList.Add(new() {
                    Name = (string)doc.GetValue("name"),
                    Icon = (string)doc.GetValue("icon")
                });
            }

            return Task.FromResult(societiesList);
        }
    }
}
