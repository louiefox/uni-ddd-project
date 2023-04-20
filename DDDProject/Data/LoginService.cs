using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DDDProject.Data
{
    public class LoginService
    {
        public Task<string> RequestLoginToken(string username, string password)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");
            var collection = database.GetCollection<BsonDocument>("Users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var studentDocument = collection.Find(filter).FirstOrDefault();

            if(studentDocument != null && studentDocument.GetValue("password") == password)
            {
                return Task.FromResult(username);
            }
            
            return Task.FromException<string>(new Exception("Invalid login information."));
        }

        public Task<string> RequestUserInfo(string loginToken)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");
            var collection = database.GetCollection<BsonDocument>("Users");

            var filter = Builders<BsonDocument>.Filter.Eq("username", loginToken);
            var studentDocument = collection.Find(filter).FirstOrDefault();

            if(studentDocument != null)
            {
                return Task.FromResult((string)studentDocument.GetValue("fullname"));
            }
            
            return Task.FromException<string>(new Exception("Invalid login token."));
        }
    }
}