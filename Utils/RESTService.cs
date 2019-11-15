using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace CuriositySoftware.Utils
{
    public class RESTService
    {
        private const string API_PREFIX = "/api/apikey/";

        public ConnectionProfile ConnectionProfile { get; }
        public string ErrorMessage { get; set; }
        public RestClient Client { get; }

        public RESTService(ConnectionProfile connectionProfile)
        {
            ConnectionProfile = connectionProfile;
            ErrorMessage = "";

            Client = new RestClient(ConnectionProfile.Url + API_PREFIX + ConnectionProfile.APIKey);
            Client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
        }

        protected T ExecRequest<T>(RestRequest request, string category) where T : new()
        {
            if (request.Resource.StartsWith(API_PREFIX))
                request.Resource = request.Resource.Remove(0, API_PREFIX.Length + ConnectionProfile.APIKey.Length);

            ErrorMessage = "";
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;

            try
            {
                IRestResponse<T> response = Client.Execute<T>(request);

                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    ErrorMessage = ("Failed : HTTP error code " + response.StatusCode.ToString() + "- " + category + " : " + response.ErrorMessage);

                    return default(T);
                }

                if (response.Data == null)
                    ErrorMessage = response.ErrorMessage;

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorMessage = ("Failed : - " + category + " : " + e.Message);

                return default(T);
            }
        }
    }
}
