using ISIFO365API.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace ISIFO365API.Controllers
{
 
    public class UserController : ApiController
    {
        private const string User = "users";
        private static GraphApp _graphApp;

        private AuthenticationContext _authContext;
        private ClientCredential _credential;
        private string _graphUrl;
        private string _version;

        public UserController()
        {
            
            this.Init();
        }
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Get()
        {
            AuthenticationResult authResult;
          
                authResult = await this._authContext.AcquireTokenAsync(this._graphUrl, this._credential);
          

            try
            {
                using (var client = new HttpClient())
                {


                    var url = $"{this._graphUrl}/{this._version}/users/Victor.Ma@ISIFDemo.onmicrosoft.com";





                    //var method = new HttpMethod("PATCH");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                    //string json = "{\"forceChangePasswordNextSignIn\": false,\"password\": \"Ma880618@\"}";
                    //request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage resultAsString = await client.SendAsync(request);
                    var result = resultAsString.Content.ReadAsStringAsync();
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //var resultAsString = await client.GetStringAsync(url);

                    //var organisation = JObject.Parse(resultAsString);
                    //var result = new JsonResult(organisation);
                    //return result;
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Init()
        {
           
            var tenant = ConfigurationManager.AppSettings["ida:Tenant"];
            var authUrl = ConfigurationManager.AppSettings["AuthUrl"];
            var authority = string.Format(authUrl, tenant);

            this._authContext = new AuthenticationContext(authority);

            var clientId = ConfigurationManager.AppSettings["ida:ClientID"];
            var clientSecret = ConfigurationManager.AppSettings["ida:Password"];

            this._credential = new ClientCredential(clientId, clientSecret);

            this._graphUrl = ConfigurationManager.AppSettings["GraphUrl"];
            this._version = ConfigurationManager.AppSettings["Version"];
        }
    }
}
