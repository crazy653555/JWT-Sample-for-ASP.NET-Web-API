using JWT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebAPI.ViewModels;

namespace WebAPI.Utils
{
    public class JWTAuthorization : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool skip = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
               || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (!skip)
            {
                string secretKey = ConfigurationManager.AppSettings["JWTSecretKey"];

                if (actionContext.Request.Headers.Authorization == null)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var token = "";

                    var tokenarray = actionContext.Request.Headers.Authorization.ToString().Split(' ');

                    if (tokenarray.Count() > 1)
                    {
                        if (tokenarray[0].Equals("Bearer"))
                        {
                            token = tokenarray[1];
                        }
                    }
                    else
                    {
                        token = tokenarray[0];
                    }

                    try
                    {
                        string jsonPayload = JsonWebToken.Decode(token, secretKey);

                        actionContext.Request.Properties.Add("userinfo", JsonConvert.DeserializeObject<JWTPayloadViewModel>(jsonPayload));
                    }
                    catch (SignatureVerificationException)
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                    catch (ArgumentException ex)
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                }
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}