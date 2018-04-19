using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace DatingAPI.Models
{
    public class KMeansData
    {
        public KMeansData()
        {
            Cluster = 0;
        }
        public ObjectId Id { get; set; }
        public int Cluster { get; set; }
        public Dictionary<String, double> Hobbies = new Dictionary<string, double>()
        {
            {"Age", 0},
            {"Reading", 0},
            {"Watching TV", 0},
            {"Movies​", 0},
            {"Fishing​", 0},
            {"Computer​", 0},
            {"Gardening​", 0},
            {"Walking​", 0},
            {"Exercise​", 0},
            {"Hunting​", 0},
            {"Shopping​", 0},
            {"Traveling​", 0},
            {"Socializing​", 0},
            {"Golf​", 0},
            {"Music​", 0},
            {"Crafts​", 0},
            {"Bicycling​", 0},
            {"Hiking​", 0},
            {"Cooking​", 0},
            {"Swimming​", 0},
            {"Camping​", 0},
            {"Skiing​", 0},
            {"Cars​", 0},
            {"Writing​", 0},
            {"Boating​", 0},
            {"Motorcycling​", 0},
            {"Bowling​", 0},
            {"Painting​", 0},
            {"Running​", 0},
            {"Dancing​", 0},
            {"Tennis​", 0},
            {"Theater​", 0},
            {"Billiards​", 0}
        };
    }
}