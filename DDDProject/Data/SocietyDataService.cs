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
                    SocietyID = (string)doc.GetValue("societyID"),
                    Name = (string)doc.GetValue("name"),
                    Icon = (string)doc.GetValue("icon")
                });
            }

            return Task.FromResult(societiesList);
        }

        public Task<List<EventData>> RequestEventsList()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var eventsCollection = database.GetCollection<BsonDocument>("Events");
            var societiesCollection = database.GetCollection<BsonDocument>("Societies");

            List<EventData> eventsList = new();
            foreach(BsonDocument doc in eventsCollection.Find(new BsonDocument()).ToList())
            {
                string societyID = (string)doc.GetValue("societyID");
                BsonDocument societyDocument = societiesCollection.Find(Builders<BsonDocument>.Filter.Eq("societyID", societyID)).FirstOrDefault();

                eventsList.Add(new() {
                    SocietyID = societyID,
                    SocietyName = (string)societyDocument.GetValue("name"),
                    Name = (string)doc.GetValue("name"),
                    DateLocation = (string)doc.GetValue("dateLocation")
                });
            }

            return Task.FromResult(eventsList);
        }

        public SocietyData FetchSocietyData(string loginToken, string societyID)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");
            var collection = database.GetCollection<BsonDocument>("Societies");

            var filter = Builders<BsonDocument>.Filter.Eq("societyID", societyID);
            var societyDoc = collection.Find(filter).FirstOrDefault();

            SocietyData societyData = new()
            {
                SocietyID = societyID,
                Name = (string)societyDoc.GetValue("name"),
                Icon = (string)societyDoc.GetValue("icon")
            };

            string username = loginToken.Replace("token_", "");

            var collectionMembers = database.GetCollection<BsonDocument>("SocietyMembers");
            var filterUsername = Builders<BsonDocument>.Filter.Eq("username", username);
            var filterSociety = Builders<BsonDocument>.Filter.Eq("societyID", societyID);

            var count = collectionMembers.Find<BsonDocument>(Builders<BsonDocument>.Filter.And(filterUsername, filterSociety)).Limit(1).CountDocuments();
            societyData.IsMemberOf = count == 1;

            BsonArray admins = (BsonArray)societyDoc.GetValue("admins");
            societyData.IsAdministrator = admins.Contains(username);

            return societyData;
        }

        public Task<SocietyData> RequestSocietyData(string loginToken, string societyID)
        {
            return Task.FromResult(FetchSocietyData(loginToken, societyID));
        }

        public Task<SocietyData> RequestJoinSociety(string loginToken, string societyID)
        {
            string username = loginToken.Replace("token_", "");
            if( username == "" )
                return Task.FromException<SocietyData>(new Exception("Invalid login token."));

            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var collectionMembers = database.GetCollection<BsonDocument>("SocietyMembers");

            var document = new BsonDocument { { "username", username }, { "societyID", societyID } };
            collectionMembers.InsertOne(document);

            return Task.FromResult(FetchSocietyData(loginToken, societyID));
        }

        public Task<SocietyData> RequestLeaveSociety(string loginToken, string societyID)
        {
            string username = loginToken.Replace("token_", "");
            if( username == "" )
                return Task.FromException<SocietyData>(new Exception("Invalid login token."));

            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var collectionMembers = database.GetCollection<BsonDocument>("SocietyMembers");

            var filterUsername = Builders<BsonDocument>.Filter.Eq("username", username);
            var filterSociety = Builders<BsonDocument>.Filter.Eq("societyID", societyID);
            collectionMembers.DeleteOne(Builders<BsonDocument>.Filter.And(filterUsername, filterSociety));

            return Task.FromResult(FetchSocietyData(loginToken, societyID));
        }

        public Task<List<SocietyData>> RequestMySocieties(string loginToken)
        {
            string username = loginToken.Replace("token_", "");
            if( username == "" )
                return Task.FromException<List<SocietyData>>(new Exception("Invalid login token."));

            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var collectionMembers = database.GetCollection<BsonDocument>("SocietyMembers");
            var collectionSocieties = database.GetCollection<BsonDocument>("Societies");

            List<SocietyData> societiesList = new();
            foreach(BsonDocument doc in collectionMembers.Find(Builders<BsonDocument>.Filter.Eq("username", username)).ToList())
            {
                string societyID = (string)doc.GetValue("societyID");
                BsonDocument societyDocument = collectionSocieties.Find(Builders<BsonDocument>.Filter.Eq("societyID", societyID)).FirstOrDefault();

                societiesList.Add(new() {
                    SocietyID = societyID,
                    Name = (string)societyDocument.GetValue("name"),
                    Icon = (string)societyDocument.GetValue("icon")
                });
            }

            return Task.FromResult(societiesList);
        }

        public Task<List<EventData>> RequestSocietyEventsList(string societyID)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var eventsCollection = database.GetCollection<BsonDocument>("Events");

            List<EventData> eventsList = new();
            foreach(BsonDocument doc in eventsCollection.Find(Builders<BsonDocument>.Filter.Eq("societyID", societyID)).ToList())
            {
                eventsList.Add(new() {
                    Name = (string)doc.GetValue("name"),
                    DateLocation = (string)doc.GetValue("dateLocation")
                });
            }

            return Task.FromResult(eventsList);
        }

        public Task<List<AnnouncementData>> RequestSocietyAnnouncements(string societyID)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var announcementsCollection = database.GetCollection<BsonDocument>("Announcements");

            List<AnnouncementData> announcementsList = new();
            foreach(BsonDocument doc in announcementsCollection.Find(Builders<BsonDocument>.Filter.Eq("societyID", societyID)).ToList())
            {
                announcementsList.Add(new() {
                    Date = (string)doc.GetValue("date"),
                    Content = (string)doc.GetValue("content")
                });
            }

            return Task.FromResult(announcementsList);
        }

        public Task<List<MemberData>> RequestSocietyMembers(string societyID)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var membersCollection = database.GetCollection<BsonDocument>("SocietyMembers");
            var usersCollection = database.GetCollection<BsonDocument>("Users");

            List<MemberData> membersList = new();
            foreach(BsonDocument doc in membersCollection.Find(Builders<BsonDocument>.Filter.Eq("societyID", societyID)).ToList())
            {
                string username = (string)doc.GetValue("username");
                BsonDocument userDocument = usersCollection.Find(Builders<BsonDocument>.Filter.Eq("username", username)).FirstOrDefault();

                membersList.Add(new() {
                    Username = username,
                    Fullname = (string)userDocument.GetValue("fullname"),
                    IsAdministrator = false
                });
            }

            return Task.FromResult(membersList);
        }

        public Task<List<EventData>> RequestSocietyCreateEvent(string societyID, string eventName, string eventDateLocation)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://admin:fDpFpTpiPwW4erIs@cluster0.ylyijxb.mongodb.net/?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("DDDProject");

            var eventsCollection = database.GetCollection<BsonDocument>("Events");

            var document = new BsonDocument { { "societyID", societyID }, { "name", eventName }, { "dateLocation", eventDateLocation } };
            eventsCollection.InsertOne(document);

            List<EventData> eventsList = new();
            foreach(BsonDocument doc in eventsCollection.Find(Builders<BsonDocument>.Filter.Eq("societyID", societyID)).ToList())
            {
                eventsList.Add(new() {
                    Name = (string)doc.GetValue("name"),
                    DateLocation = (string)doc.GetValue("dateLocation")
                });
            }

            return Task.FromResult(eventsList);
        }
    }
}
