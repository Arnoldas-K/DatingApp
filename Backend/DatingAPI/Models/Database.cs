using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Microsoft.Owin;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatingAPI.Models
{
    public class Database
    {
        IMongoDatabase _database;

        /* Initialising the database */
        public Database()
        {
            var connectUrl = "mongodb://localhost:27017";
            //var connectUrl = "mongodb://arnoldask:bbd.blt96@mdatingdb-dy6nk.mongodb.net:27017/DatingDB?ssl=true&replicaSet=rs0&authSource=arnoldask";
            MongoClient mongoClient = new MongoClient(connectUrl);
/*            string connectionString =
                @"mongodb://datingdb:lysHeiIhqBPNalqIH811pB9i0lqKoJhupKE3PsrsQRTBRkSUCLwGlaoXjmk4Cm92l7MoC1AVumWqgseBH1NdCA==@datingdb.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
            settings.SslSettings =
                new SslSettings() {EnabledSslProtocols = SslProtocols.Tls12};
            var mongoClient = new MongoClient(settings);*/
            _database = mongoClient.GetDatabase("DatingDB");
        }

        // Return user's model from the database, for further operations
        public async Task<UserModel> GetUser(string id)
        {
            return await _database.GetCollection<UserModel>("Users").Find(c => c.Id.Equals(new ObjectId(id)))
                .FirstOrDefaultAsync();
        }

        // Data used in user's profile
        public async Task<ProfileModel> GetProfile(string id)
        {
            var user = await GetUser(id);
            ProfileModel returnInfo = new ProfileModel
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                LastName = user.LastName,
                Age = user.Age,
                Gender = user.Gender,
                Location = user.Location,
                Occupation = user.Occupation,
                About = user.About,
                Interests = user.Interests,
                Photo = user.Photo,
                //Email = user.Email,
                //FromAge = user.FromAge,
                //ToAge = user.ToAge,
                LookFor = user.LookFor
            };
            return returnInfo;
        }

        // for testing
        public async Task<List<UserModel>> GetUsers()
        {
            return await _database.GetCollection<UserModel>("Users").Find(_ => true).ToListAsync();
        }

        public async Task RemoveData()
        {
            await _database.DropCollectionAsync("Centroids");
            await _database.DropCollectionAsync("Clusterized");
            await _database.DropCollectionAsync("KMeans");
        }

        public async Task<bool> Register(UserModel model)
        {
            var exists = await GetUser(model.Id.ToString());
            if (exists != null)
            {
                return false;
            }
            else
            {
                if (model.Photo == null) // for testing
                {
                    if (model.Gender.Equals("Male"))
                    {
                        model.Photo = "http://localhost:56761/webroot/test0.jpeg";
                    }
                    else
                    {
                        model.Photo = "http://localhost:56761/webroot/test1.jpg";
                    }
                }
                var users = await GetUsers();
                if (users.Count > 300) // if there sufficient amount of users, search for clusters // default cluster - 0
                {
                    model.Cluster = await FindClosestCentroidAsync(model);
                }
                await _database.GetCollection<UserModel>("Users").InsertOneAsync(model);
                return true;
            }
        }

        public async Task InsertCentroids(KMeansData centroids)
        {
            await _database.GetCollection<KMeansData>("Centroids").InsertOneAsync(centroids);
        }

        public async Task StandardiseData()
        {
            //await RemoveData();
            List<KMeansData> data = new List<KMeansData>();
            var users = await GetUsers();
            // (age - average) / range (max-min) // interests

            // AGE STANDARDISE //
            double ageSum = 0;
            double maxAge = Double.MinValue, minAge = Double.MaxValue;
            foreach (var user in users)
            {
                if (maxAge < user.Age) maxAge = user.Age;
                if (minAge > user.Age) minAge = user.Age;
                ageSum += user.Age;
            }
            double avgAge = ageSum / users.Count;
            double range = maxAge - minAge;
            // AGE //
            double interestRange = 1; // max 1, min 0 // FIND RANGE OF EACH INTEREST
            KMeansData sumOfInterests = new KMeansData();
            KMeansData avgOfInterests = new KMeansData();
            foreach (var user in users)
            {
                //ages.Add((user.Age - ageMean)/ageSD);
                KMeansData kuser = new KMeansData { Id = user.Id, Hobbies = { ["Age"] = (user.Age - avgAge) / range } };
                foreach (var userInterest in user.Interests)
                {
                    kuser.Hobbies[userInterest] = 1;
                    sumOfInterests.Hobbies[userInterest] += 1;
                }
                data.Add(kuser);
                //await _database.KMeansInsert(kuser);
            }

            // average for each hobby
            foreach (var interest in sumOfInterests.Hobbies.Keys)
            {
                avgOfInterests.Hobbies[interest] = sumOfInterests.Hobbies[interest] / users.Count;
            }

            // finalizing the data
            foreach (var kMeansData in data)
            {
                foreach (var hobby in avgOfInterests.Hobbies.Keys)
                {
                    kMeansData.Hobbies[hobby] =
                        (kMeansData.Hobbies[hobby] - avgOfInterests.Hobbies[hobby]) / interestRange;
                }
                await KMeansInsert(kMeansData);
            }
        }

        public async Task Clusterize()
        {
            List<KMeansData> data = await GetKMeansData();
            List<KMeansData> centroids = await AnomalousPatterns();
            data = await ClusterizeAsync();

            data.Sort((s1, s2) => s1.Cluster.CompareTo(s2.Cluster));
            foreach (var kMeansData in data)
            {
                await ClusterisedInsert(kMeansData);
            }

            foreach (var centroid in centroids)
            {
                await InsertCentroids(centroid);
            }

            async Task<List<KMeansData>> AnomalousPatterns()
            {
                // 1. Count distance
                // 2. Assign clusters to furthest centroid and center
                // 3. Remove if belongs to furthest
                List<KMeansData> centroidsList = new List<KMeansData>();
                List<KMeansData> tempData = await GetKMeansData();
                KMeansData center = new KMeansData(); //0,0,0,0,0,0...
                KMeansData furthest = new KMeansData();

                while (tempData.Count != 0)
                {
                    furthest = FindFurthestCluster(tempData, furthest);
                    tempData.Remove(furthest);
                    tempData = AssignClusters(new List<KMeansData> { center, furthest }, tempData);
                    if (tempData.Count(x => x.Cluster == 1) >= 4)
                    {
                        centroidsList.Add(furthest); // if centroid has more than two members add to centroids list
                        tempData.RemoveAll(x => x.Cluster == 1); // 1 stands for furthest, remove if it belongs to it
                    }
                }
                return centroidsList;
            }

            async Task<List<KMeansData>> ClusterizeAsync()
            {
                int maxCycles = 30;
                for (int i = 0; i < maxCycles; i++)
                {
                    data = AssignClusters(centroids, data);
                    UpdateCentroids(ref centroids, data, out var updated);
                    if (!updated) break;
                }
                return data;
            }

            bool CompareDictionaries(Dictionary<string, double> x, Dictionary<string, double> y)
            {
                foreach (var xKey in x.Keys)
                {
                    if (!x[xKey].Equals(y[xKey]))
                    {
                        return false;
                    }
                }
                return true;
            }

            KMeansData FindFurthestCluster(List<KMeansData> tempData, KMeansData furthest)
            {
                KMeansData center = new KMeansData();
                double furthestCluster = Double.MinValue;
                foreach (var point in tempData)
                {
                    double dist = 0;
                    foreach (var pointHobby in point.Hobbies)
                    {
                        double hobbyDist = Math.Pow((pointHobby.Value - center.Hobbies[pointHobby.Key]), 2);
                        dist += hobbyDist;
                    }
                    if (dist > furthestCluster)
                    {
                        furthestCluster = dist;
                        furthest = point;
                    }
                }
                return furthest;
            }

            List<KMeansData> AssignClusters(List<KMeansData> centroidsList, List<KMeansData> clusters)
            {
                foreach (var cluster in clusters)
                {
                    // Find distances between cluster and existing centroids
                    double[] distancesFromCentroids = new double[centroidsList.Count];
                    for (int i = 0; i < centroidsList.Count; i++)
                    {
                        double dist = 0;
                        foreach (var clusterHobby in cluster.Hobbies)
                        {
                            dist += Math.Pow((clusterHobby.Value - centroidsList[i].Hobbies[clusterHobby.Key]), 2);
                        }
                        distancesFromCentroids[i] = dist;
                    }

                    // Which is the nearest?
                    int centroidWithMin = 0;
                    for (int i = centroidWithMin + 1; i < distancesFromCentroids.Length; i++)
                    {
                        if (distancesFromCentroids[centroidWithMin] > distancesFromCentroids[i])
                        {
                            centroidWithMin = i;
                        }
                    }
                    cluster.Cluster = centroidWithMin;
                }
                return clusters;
            }

            void UpdateCentroids(ref List<KMeansData> centroidsList, List<KMeansData> clusters, out bool updated)
            {
                updated = false;
                for (int i = 0; i < centroidsList.Count; i++)
                {
                    List<KMeansData> membersOfCluster = clusters.FindAll(member => member.Cluster == i);
                    KMeansData sumOfData = new KMeansData();
                    for (int j = 0; j < membersOfCluster.Count; j++)
                    {
                        foreach (var hobbiesKey in centroidsList[0].Hobbies.Keys)
                        {
                            sumOfData.Hobbies[hobbiesKey] += membersOfCluster[j].Hobbies[hobbiesKey];
                        }
                    }
                    KMeansData averageCluster = new KMeansData();
                    foreach (var hobbiesKey in clusters[0].Hobbies.Keys)
                    {
                        averageCluster.Hobbies[hobbiesKey] = sumOfData.Hobbies[hobbiesKey] / membersOfCluster.Count;
                    }
                    if (!updated)
                    {
                        if (!CompareDictionaries(centroidsList[i].Hobbies, averageCluster.Hobbies))
                        {
                            updated = true;
                        }
                    }
                    centroidsList[i] = averageCluster;
                }
            }
        }

        public async Task<List<UserModel>> FindClosestCentroidListAsync()
        {
            List<KMeansData> centroids =
                await _database.GetCollection<KMeansData>("Centroids").Find(_ => true).ToListAsync();
            List<UserModel> users = await GetUsers();
            // ADD STANDARDISATION
            return await AssignClustersListAsync(centroids, users);
            async Task<List<UserModel>> AssignClustersListAsync(List<KMeansData> centroidsList, List<UserModel> clusters)
            {
                foreach (var cluster in clusters)
                {
                    KMeansData standardisedData = await StandardiseData(cluster);
                    // Find distances between cluster and existing centroids
                    double[] distancesFromCentroids = new double[centroidsList.Count];
                    for (int i = 0; i < centroidsList.Count; i++)
                    {
                        double dist = 0;
                        foreach (var clusterHobby in standardisedData.Hobbies)
                        {
                            dist += Math.Pow((clusterHobby.Value - centroidsList[i].Hobbies[clusterHobby.Key]), 2);
                        }
                        distancesFromCentroids[i] = dist;
                    }

                    // Which is the nearest?
                    int centroidWithMin = 0;
                    for (int i = centroidWithMin + 1; i < distancesFromCentroids.Length; i++)
                    {
                        if (distancesFromCentroids[centroidWithMin] > distancesFromCentroids[i])
                        {
                            centroidWithMin = i;
                        }
                    }
                    cluster.Cluster = centroidWithMin;
                }
                return clusters;
            }
        }

        public async Task<int> FindClosestCentroidAsync(UserModel model)
        {
            List<KMeansData> centroids =
                await _database.GetCollection<KMeansData>("Centroids").Find(_ => true).ToListAsync();
            KMeansData standardisedData = await StandardiseData(model);
            // ADD STANDARDISATION

            int result = 0;
            double closestCentroid = Double.MaxValue;
            for (int i = 0; i < centroids.Count; i++)
            {
                double dist = 0;
                foreach (var pointHobby in standardisedData.Hobbies)
                {
                    double hobbyDist = Math.Pow((pointHobby.Value - centroids[i].Hobbies[pointHobby.Key]), 2);
                    dist += hobbyDist;
                }
                if (dist < closestCentroid)
                {
                    closestCentroid = dist;
                    result = i;
                }
            }
            return result;
        }

        async Task<KMeansData> StandardiseData(UserModel model)
        {
            //TODO: range age, avg age | max 1 min 0 avg of interests
            KMeansStatistics statistics = await GetStatistics();

            KMeansData kuser = new KMeansData
            {
                Id = model.Id,
                Hobbies = {["Age"] = (model.Age - statistics.AvgAge) / statistics.AgeRange}
            };
            foreach (var userInterest in model.Interests)
            {
                kuser.Hobbies[userInterest] = 1;
            }
            //await _database.KMeansInsert(kuser);

            // finalizing the data
            KMeansData sample = new KMeansData();
            foreach (var hobby in sample.Hobbies.Keys)
            {
                kuser.Hobbies[hobby] =
                    (kuser.Hobbies[hobby] - statistics.HobbiesAverages[hobby]) / statistics.HobbiesRange;
            }
            return kuser;
        }

        async Task<KMeansStatistics> GetStatistics()
        {
            var users = await GetUsers();
            KMeansStatistics statistics = new KMeansStatistics();
            //if(await _database.GetCollection<KMeansStatistics>("KMeansStatistics").Find(_ => true).FirstOrDefaultAsync() != null) return await _database.GetCollection<KMeansStatistics>("KMeansStatistics").Find(_ => true).FirstOrDefaultAsync();
            if (users.Count != 0 && users.Count % 300 == 0) // testing purposes / update statistics each 300 users to get accurate centroids
            {
                
                statistics.AgeRange = _database.GetCollection<UserModel>("Users").Find(_ => true)
                                          .Sort(Builders<UserModel>.Sort.Descending("Age")).FirstOrDefault().Age -
                                      _database.GetCollection<UserModel>("Users").Find(_ => true)
                                          .Sort(Builders<UserModel>.Sort.Ascending("Age")).FirstOrDefault().Age;
                double avgAge = 0;
                foreach (var userModel in users)
                {
                    avgAge += userModel.Age;
                }
                statistics.AvgAge = avgAge / users.Count;

                // Hobbies average
                KMeansData sumOfInterests = new KMeansData();
                foreach (var user in users)
                {
                    foreach (var userInterest in user.Interests)
                    {
                        sumOfInterests.Hobbies[userInterest] += 1;
                    }
                }

                // average for each hobby
                foreach (var interest in sumOfInterests.Hobbies.Keys)
                {
                    statistics.HobbiesAverages[interest] = sumOfInterests.Hobbies[interest] / users.Count;
                }
                await _database.DropCollectionAsync("KMeansStatistics");
                await _database.GetCollection<KMeansStatistics>("KMeansStatistics").InsertOneAsync(statistics);
                return statistics;
            }
            else
            {
                return await _database.GetCollection<KMeansStatistics>("KMeansStatistics").Find(_ => true).FirstOrDefaultAsync();
            }
        }

        public async Task KMeansInsert(KMeansData model)
        {
            await _database.GetCollection<KMeansData>("KMeans").InsertOneAsync(model);
        }

        public async Task ClusterisedInsert(KMeansData model)
        {
            await _database.GetCollection<KMeansData>("Clusterized").InsertOneAsync(model);
        }

        public async Task<List<KMeansData>> GetKMeansData()
        {
            return await _database.GetCollection<KMeansData>("KMeans").Find(_ => true).ToListAsync();
        }

        // Authorization
        public async Task<UserModel> Login(string username, string password)
        {
            var exists = await _database.GetCollection<UserModel>("Users").Find(m => m.Login.Equals(username))
                .FirstOrDefaultAsync();
            if (exists != null)
            {
                if (exists.Password.Equals(password))
                {
                    return exists;
                }
            }
            return null;
        }

        // Update user data
        public async Task<ReplaceOneResult> Update(UserModel model)
        {
            return await _database.GetCollection<UserModel>("Users").ReplaceOneAsync(e => e.Id.Equals(model.Id), model);
        }

        // Update conversation data
        public async Task<ReplaceOneResult> UpdateConversation(Conversation model)
        {
            return await _database.GetCollection<Conversation>("Conversations")
                .ReplaceOneAsync(e => e.Id.Equals(model.Id), model);
        }

        private async Task<Conversation> CreateConversation(UserModel user, UserModel partner)
        {
            // creating a conversation
            Conversation conversation = new Conversation();
            conversation.Messages = new List<Conversation.Message>
            {
                // adding first greeting message
                new Conversation.Message
                {
                    From = "Server",
                    Text = "You have matched.",
                    Time = DateTime.Now.ToString("G")
                }
            };
            conversation.UserNames = new List<string>
            {
                user.Name,
                partner.Name
            };
            conversation.Users = new List<string>
            {
                user.Id.ToString(),
                partner.Id.ToString()
            };
            conversation.Photos = new List<string>
            {
                user.Photo,
                partner.Photo
            };
            // storing in the database
            await _database.GetCollection<Conversation>("Conversations").InsertOneAsync(conversation);
            return conversation;
        }

        /* Like function. Add suggestion's id to the user's likes list, create conversation if /
        / both - user and partner have each other liked */
        public async Task<ReplaceOneResult> Like(string userid, string partnerid)
        {
            // filters
            var user = await GetUser(userid);
            var partner = await GetUser(partnerid);
            if (user != null && partner != null)
            {
                // add like
                if (user.Likes == null) user.Likes = new List<string>();
                if (partner.Likes == null) partner.Likes = new List<string>();
                // check if the liked user, has showed interest as well
                // if yes, create a conversation
                user.Likes.Add(partnerid);
                if (partner.Likes.Contains(userid))
                {
                    Conversation conversation = await CreateConversation(user, partner);

                    // initialising conversations if there's none
                    if (partner.Conversations == null) partner.Conversations = new List<string>();
                    if (user.Conversations == null) user.Conversations = new List<string>();

                    // adding
                    user.Conversations.Add(conversation.Id.ToString());
                    partner.Conversations.Add(conversation.Id.ToString());

                    await Update(partner);
                }
            }
            return await Update(user);
        }

        // Dislike function
        public async Task<ReplaceOneResult> Dislike(string userid, string partnerid)
        {
            var user = await GetUser(userid);
            var partner = await GetUser(partnerid);
            if (user != null && partner != null)
            {
                if (user.Dislikes == null) user.Dislikes = new List<string>();
                user.Dislikes.Add(partnerid);
            }
            return await Update(user);
        }

        // GET A SUGGESTION FUNCTION
        public async Task<SearchModel> GetSuggestion(string userId)
        {
            var user = await GetUser(userId);
            var filter = Builders<UserModel>.Filter.Eq(e => e.Gender, user.LookFor) &
                         Builders<UserModel>.Filter.Eq(e => e.Cluster, user.Cluster) &
                         //Builders<UserModel>.Filter.Lte(e => e.Age, user.ToAge) &
                         //Builders<UserModel>.Filter.Gte(e => e.Age, user.FromAge) &
                         //Builders<UserModel>.Filter.Gte(e => e.ToAge, user.Age) &
                         //Builders<UserModel>.Filter.Lte(e => e.FromAge, user.Age) &
                         Builders<UserModel>.Filter.Eq(e => e.LookFor, user.Gender);
            var suggestions = await _database.GetCollection<UserModel>("Users").Find(filter).ToListAsync();
            foreach (UserModel suggestion in suggestions)
            {
                ////////////////////////////////////////////////////////////////
                // INITIALISE IN THE REGISTRATION
                if (user.Likes == null) user.Likes = new List<string>();
                if (user.Dislikes == null) user.Dislikes = new List<string>();
                //
                ////////////////////////////////////////////////////////////////
                if (!user.Likes.Contains(suggestion.Id.ToString()) &&
                    !user.Dislikes.Contains(suggestion.Id.ToString()) && user.Id.ToString() != suggestion.Id.ToString())
                {
                    // returning essential information
                    SearchModel returnInfo = new SearchModel
                    {
                        Id = suggestion.Id.ToString(),
                        Name = suggestion.Name,
                        Age = suggestion.Age,
                        Gender = suggestion.Gender,
                        Location = suggestion.Location,
                        Occupation = suggestion.Occupation,
                        About = suggestion.About,
                        Interests = suggestion.Interests,
                        Photo = suggestion.Photo
                    };

                    return returnInfo;
                }
            }
            return null;
        }

        // RETRIEVE AVAILABLE CONVERSATIONS
        public async Task<List<Conversation>> GetAllConversations(string userId)
        {
            UserModel user = await GetUser(userId);
            List<Conversation> result = new List<Conversation>();


            // retrieving conversation for each id
            // adding important details, and last message of that conversation
            // to the result list
            if (user.Conversations != null)
            {
                if (user.Conversations[0] != null)
                {
                    foreach (string userConversation in user.Conversations)
                    {
                        var conversation = await GetConversation(userConversation);
                        var dataForReturn = new Conversation
                        {
                            Id = conversation.Id,
                            Messages = new List<Conversation.Message>
                            {
                                conversation.Messages[conversation.Messages.Count - 1]
                            },
                            UserNames = conversation.UserNames,
                            Users = conversation.Users
                            // Photos = conversation.Photos
                        };
                        var fphoto = await GetUser(dataForReturn.Users[0]);
                        var sphoto = await GetUser(dataForReturn.Users[1]);
                        dataForReturn.Photos = new List<string>
                        {
                            fphoto.Photo,
                            sphoto.Photo
                        };
                        result.Add(dataForReturn);
/*
                        result.Add(new Conversation
                        {
                            Id = conversation.Id,
                            Messages = new List<Conversation.Message>
                            {
                                conversation.Messages[conversation.Messages.Count - 1]
                            },
                            UserNames = conversation.UserNames,
                            Users = conversation.Users,
                           // Photos = conversation.Photos
                           Photos = new List<string>()
                           {
                               await GetUser(conversation.Users[0]).Result.Photo,
                               await GetUser(conversation.Users[1]).Result.Photo
                           }
                        });*/
                    }
                    return result;
                }
            }
            return null;
        }

        // Retrieve messages of the selected conversation
        public async Task<Conversation> GetConversation(string id)
        {
            return await _database.GetCollection<Conversation>("Conversations").Find(c => c.Id.Equals(new ObjectId(id)))
                .FirstOrDefaultAsync();
        }

        /* Send message function, which is called after 'ChatHub' function 'Send' is invoked.
         * A new message is added to the conversation's messages. */
        public async Task<ReplaceOneResult> SendMessage(string conversationId, string from, string message)
        {
            var conversation = await GetConversation(conversationId);
            conversation?.Messages.Add(new Conversation.Message
            {
                From = @from,
                Text = message,
                Time = DateTime.Now.ToString("G")
            });
            return await UpdateConversation(conversation);
        }
    }
}