using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;

namespace DatingAPI.Models
{
    public class UserModel
    {
        public ObjectId Id { get; set; }
        [Required]
        public String Login { get; set; }
/*        [EmailAddress]
        public String Email { get; set; }*/
        [Required]
        public String Password { get; set; }
        public String Photo { get; set; }
        public String Name { get; set; }
        public String LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public String Location { get; set; }
        public String Occupation { get; set; }
        public String About { get; set; }
        public List<String> Interests { get; set; } // which gender interested

        //search options
/*        public int FromAge { get; set; }
        public int ToAge { get; set; }*/
        public String LookFor { get; set; }

        //search results
        public int Cluster { get; set; }
        public List<String> Likes { get; set; } // ids of liked people for matching
        public List<String> Dislikes { get; set; } // ids of disliked to avoid future suggesting
        public List<String> Conversations { get; set; } // ids of conversations with matched people

        public UserModel()
        {
            Cluster = 0; // on initialising
        }
    }
}