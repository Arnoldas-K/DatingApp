using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;
using DatingAPI.Controllers;
using DatingAPI.Models;
using Microsoft.Owin.Security.OAuth;

namespace DatingAPI
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            DataController data = new DataController();
            var user = await data.Login(context.UserName, context.Password);
            if(user != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.SerialNumber, user.Id.ToString()));
                //identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                identity.AddClaim(new Claim("Username", user.Login));
                context.Validated(identity);
            }
            else
            {
                context.SetError("Invalid data", "Incorrect username or password");
                return;
            }
        }
    }
}