using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace DatingAPI
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }

        //        protected void Application_BeginRequest()
        //        {
        //            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
        //            {
        //                Response.Flush();
        //            }
        //        }

        protected void Application_BeginRequest()
        {
            //            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            //            {
            //                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
            //                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS, POST");
            //                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");
            //                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
            //                HttpContext.Current.Response.End();
            //            }
            var res = HttpContext.Current.Response;
            var req = HttpContext.Current.Request;
            res.AppendHeader("Access-Control-Allow-Origin", "*");
            res.AppendHeader("Access-Control-Allow-Credentials", "true");
            res.AppendHeader("Access-Control-Allow-Headers", "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Authorization, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
            res.AppendHeader("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");

            // ==== Respond to the OPTIONS verb =====
            if (req.HttpMethod == "OPTIONS")
            {
                res.StatusCode = 200;
                res.End();
            }
        }

    }
}