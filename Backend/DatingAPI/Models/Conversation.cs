using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;

namespace DatingAPI.Models
{
    public class Conversation
    {
        public ObjectId Id { get; set; } // chat id
        public List<Message> Messages { get; set; }
        public List<String> Photos { get; set; }
        public List<String> UserNames;
        public List<String> Users;
        public class Message
        {
            public String Text { get; set; }
            public String From { get; set; }
            //public String To { get; set; }
            public String Time { get; set; }
        }
    }
}