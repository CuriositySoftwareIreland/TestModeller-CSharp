using CuriositySoftware.JobEngine.Entities;
using CuriositySoftware.Utilities;
using CuriositySoftware.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.JobEngine.Services
{
    public class JobSubmissionService
    {
        public ConnectionProfile ConProfile;

        public JobSubmissionService(ConnectionProfile cnProfile)
        {
            this.ConProfile = cnProfile;
        }

        public JobEntity GetJob(long jobId)
        {
            RestClient client = new RestClient(ConProfile.Url);
            client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

            RestRequest request = new RestRequest("/api/apikey/" + ConProfile.APIKey + "/job/" + jobId, Method.GET);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;

            try
            {
                IRestResponse<JobEntity> response = client.Execute<JobEntity>(request);

                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    Console.WriteLine("Failed : HTTP error code - GetJob : " + response.Content);
                }

                return response.Data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed : - GetJob : " + e.Message);
            }

            return null;
        }
    }
}
