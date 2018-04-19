using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingAPI.Models
{
    // Model used for the search function
    public class SearchModel
    {
        public String Id { get; set; }
        public String Photo { get; set; }
        public String Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public String Location { get; set; }
        public String Occupation { get; set; }
        public String About { get; set; }
        public List<String> Interests { get; set; } // which gender interested
    }
}