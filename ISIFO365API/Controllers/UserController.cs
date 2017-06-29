using ISIFO365API.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        
        private static GraphApp _graphApp;

        private AuthenticationContext _authContext;
        private ClientCredential _credential;
        private string _graphUrl;
        private string _version;
        private List<UserEntity> userEntity;

        public UserController()
        {
            
            this.Init();
        }
        [System.Web.Mvc.HttpGet]
        public async Task<string> Get(string id)
        {

            userEntity = new List<UserEntity>();
            AuthenticationResult authResult;
          
                authResult = await this._authContext.AcquireTokenAsync(this._graphUrl, this._credential);
            

            try
            {
                using (var client = new HttpClient())
                {
                   var userList=  await GetUsersList(authResult, client);
                    var currentUser = new UserEntity();
                    foreach(UserEntity u in userList)
                    {
                        if (u.givenName == id)
                        {
                            currentUser = u;
                        }
                    }


                    var url = $"{this._graphUrl}/{this._version}/users/{currentUser.mail}";





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
                    return result.Result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<UserEntity>> GetUsersList(AuthenticationResult authResult, HttpClient client)
        {
            var searchAll = $"{this._graphUrl}/{this._version}/users";
            HttpRequestMessage searchRequest = new HttpRequestMessage(HttpMethod.Get, searchAll);
            searchRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            HttpResponseMessage resultUser = await client.SendAsync(searchRequest);
            var userResult = resultUser.Content.ReadAsStringAsync().Result.ToString();

            JObject o = JObject.Parse(userResult);
            JArray jlist = JArray.Parse(o["value"].ToString());
            for (int i = 0; i < jlist.Count; i++)
            {
                UserEntity u = new UserEntity();
                var temp = JObject.Parse(jlist[i].ToString());
                u.businessPhones = !string.IsNullOrEmpty(temp["businessPhones"].ToString()) ? temp["businessPhones"].ToString() : "";
                u.displayName = !string.IsNullOrEmpty(temp["displayName"].ToString()) ? temp["displayName"].ToString() : "";
                u.givenName = !string.IsNullOrEmpty(temp["givenName"].ToString()) ? temp["givenName"].ToString() : "";
                u.id = !string.IsNullOrEmpty(temp["id"].ToString()) ? temp["id"].ToString() : "";
                u.jobTitle = !string.IsNullOrEmpty(temp["jobTitle"].ToString()) ? temp["jobTitle"].ToString() : "";
                u.mail = !string.IsNullOrEmpty(temp["mail"].ToString()) ? temp["mail"].ToString() : "";
                u.mobilePhone = !string.IsNullOrEmpty(temp["mobilePhone"].ToString()) ? temp["mobilePhone"].ToString() : "";
                u.officeLocation = !string.IsNullOrEmpty(temp["officeLocation"].ToString()) ? temp["officeLocation"].ToString() : "";
                u.preferredLanguage = !string.IsNullOrEmpty(temp["preferredLanguage"].ToString()) ? temp["preferredLanguage"].ToString() : "";
                u.surname = !string.IsNullOrEmpty(temp["surname"].ToString()) ? temp["surname"].ToString() : "";
                u.userPrincipalName = !string.IsNullOrEmpty(temp["userPrincipalName"].ToString()) ? temp["userPrincipalName"].ToString() : "";
                userEntity.Add(u);
            }
            return userEntity;
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
