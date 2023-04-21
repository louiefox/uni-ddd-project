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
                    Fullname = (string)studentDocument.GetValue("fullname"),
                    ProfileBio = (string)studentDocument.GetValue("profileBio"),
                    ProfileDepartment = (string)studentDocument.GetValue("profileDepartment")
                };

                return Task.FromResult(user);
            }
            
            return Task.FromException<UserInfo>(new Exception("Invalid login token."));
        }

        public Task<string> RequestSetProfile(string loginToken, string profileDepartment, string profileBio)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");
            var collection = database.GetCollection<BsonDocument>("Users");

            string username = loginToken.Replace("token_", "");
            var filter = Builders<BsonDocument>.Filter.Eq("username", username);

            var update = Builders<BsonDocument>.Update.Set("profileDepartment", profileDepartment);
            collection.UpdateOne(filter, update);

            var update2 = Builders<BsonDocument>.Update.Set("profileBio", profileBio);
            collection.UpdateOne(filter, update2);

            return Task.FromException<string>(new Exception("Invalid login token."));
        }
    }
}
