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
                return Task.FromResult("token_" + username);
            }
            
            return Task.FromException<string>(new Exception("Invalid login information."));
        }

        public Task<UserInfo> RequestUserInfo(string loginToken)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");
            var collection = database.GetCollection<BsonDocument>("Users");

            string username = loginToken.Replace("token_", "");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);
            var studentDocument = collection.Find(filter).FirstOrDefault();

            if(studentDocument != null)
            {
                UserInfo user = new()
                {
                    Username = username,
                    Fullname = (string)studentDocument.GetValue("fullname")
                };

                return Task.FromResult(user);
            }
            
            return Task.FromException<UserInfo>(new Exception("Invalid login token."));
        }
    }
}
