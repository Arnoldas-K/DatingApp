using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using DatingAPI.Models;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using MongoDB.Driver;

namespace DatingAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DataController : ApiController
    {
        private readonly Database _database;

        public DataController()
        {
            _database = new Database();
        }

        /*Return an ID for the user*/
        [Authorize]
        [HttpGet]
        [Route("api/data/authenticate")]
        public IHttpActionResult GetForAuthenticate()
        {
            var identity = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.SerialNumber);
            return Ok(identity);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/data/test")]
        public async Task<IHttpActionResult> CreateSampleData()
        {
            String[] hobbies = new String[]
            {
                "Reading", "Watching TV", "Movies​", "Fishing​", "Computer​",
                "Gardening​", "Walking​", "Exercise​", "Hunting​", "Shopping​",
                "Traveling​", "Socializing​", "Golf​", "Music​",
                "Crafts​", "Bicycling​", "Hiking​", "Cooking​",
                "Swimming​", "Camping​", "Skiing​", "Cars​", "Writing​", "Boating​", "Motorcycling​", "Bowling​",
                "Painting​", "Running​", "Dancing​", "Tennis​", "Theater​", "Billiards​",
            };

            for (int i = 0; i < 300; i++)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                int age = random.Next(18, 40);
                String gender = random.Next(0, 2) == 0 ? "Male" : "Female";
                String lookfor = gender.Equals("Male") ? "Female" : "Male";
                String photo = gender.Equals("Male") ?  "test0.jpeg" : "test1.jpg";

                List<String> interests = new List<string>();
                int interestsamount = random.Next(2, 7); // up to 6 hobbies
                for (int j = 0; j < interestsamount; j++)
                {
                    int hobby = random.Next(0, hobbies.Length);
                    if (!interests.Contains(hobbies[hobby]))
                    {
                        interests.Add(hobbies[hobby]);
                    }
                }
                // LOOK FOR

                var user = new UserModel
                {
                    Login = "test" + i,
                    //Email = "test@test.com",
                    Password = "test",
                    Photo = "http://localhost:56761/webroot/" + photo,
                    Name = "Test" + i,
                    LastName = "Tester",
                    Age = age, //rand
                    Gender = gender, // rand
                    Location = "Colchester, UK",
                    Occupation = "Student at University of Essex",
                    About = "Short info about me.",
                    Interests = interests, // RAND
                    //FromAge = fromage, // rand
                    //ToAge = toage, // rand,
                    //Cluster = findCluster(), // implement cluster 
                    LookFor = lookfor // rand
                };

                await _database.Register(user);
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/data/testdata")]
        public async Task<IHttpActionResult> ProcessData()
        {
            await _database.RemoveData();
            await _database.StandardiseData();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/data/testclusterize")]
        public async Task<IHttpActionResult> Clusterize()
        {
            await _database.Clusterize();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/data/usersclusterize")]
        public async Task AssignToClusters()
        {
            var clusterisedUsers = await _database.FindClosestCentroidListAsync();
            foreach (var clusterisedUser in clusterisedUsers)
            {
                await _database.Update(clusterisedUser);
            }
        }


        /*Registration*/
        [AllowAnonymous]
        [HttpPost]
        [Route("api/data/register")]
        public async Task<bool> Register(UserModel model)
        {
            return await _database.Register(model);
        }

        /*Login, received username and password, validated data in Database class*/
        [Authorize]
        [HttpPost]
        [Route("api/data/login")]
        public async Task<UserModel> Login(string login, string password)
        {
            if (login != null && password != null)
            {
                return await _database.Login(login, password);
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("api/data/like")]
        public async Task<ReplaceOneResult> Like(RequestModel model)
        {
            return await _database.Like(model.Id, model.Partnerid);
        }

        [Authorize]
        [HttpPost]
        [Route("api/data/dislike")]
        public async Task<ReplaceOneResult> Dislike(RequestModel model)
        {
            return await _database.Dislike(model.Id, model.Partnerid);
        }

        [Authorize]
        [HttpPost]
        [Route("api/data/suggestion")]
        public async Task<SearchModel> GetSuggestion(RequestModel model)
        {
            return await _database.GetSuggestion(model.Id);
        }

        // For retrieving a list of available conversations with some data
        [Authorize]
        [HttpPost]
        [Route("api/data/conversations")]
        public async Task<List<Conversation>> GetConversations(RequestModel model)
        {
            return await _database.GetAllConversations(model.Id);
        }

        [Authorize]
        [HttpPost]
        [Route("api/data/conversation")]
        public async Task<Conversation> GetConversation(RequestModel model)
        {
            return await _database.GetConversation(model.Id);
        }

        [Authorize]
        [HttpPost]
        [Route("api/data/user")]
        public async Task<ProfileModel> GetUser(RequestModel model)
        {
            return await _database.GetProfile(model.Id);
        }

        [Authorize]
        [HttpPost]
        [Route("api/data/upload")]
        public async Task<IHttpActionResult> UploadPhoto()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }
            var provider = new MultipartMemoryStreamProvider();
            string root = System.Web.HttpContext.Current.Server.MapPath("~/webroot/");
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var identity = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.SerialNumber);
                var extension = filename.Substring(filename.IndexOf('.'));
                //var extension = filename.Substring(filename.Length - 4);
                var finalName = identity.Value + extension;
                byte[] fileArray = await file.ReadAsByteArrayAsync();

                using (System.IO.FileStream fs = new System.IO.FileStream(root + finalName, System.IO.FileMode.Create))
                {
                    await fs.WriteAsync(fileArray, 0, fileArray.Length);
                }

                var user = await _database.GetUser(identity.Value);
                //var physicalFileSystem = new PhysicalFileSystem(Path.Combine(root, "webroot"));
                /*user.Photo = "https://datingapi20180316115426.azurewebsites.net/webroot/" + finalName;*/
                user.Photo = "http://localhost:56761/webroot/" + finalName;
                await _database.Update(user);
            }
            return Ok("File uploaded");
        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage {StatusCode = HttpStatusCode.OK};
        }
    }
}