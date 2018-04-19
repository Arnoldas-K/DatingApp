using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingAPI.Models
{
    // Model used for user profile
    public class ProfileModel
    {
        public String Id { get; set; }
        public String Email { get; set; }
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
        //public int FromAge { get; set; }
        //public int ToAge { get; set; }
        public String LookFor { get; set; }
    }
}