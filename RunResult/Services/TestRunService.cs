using CuriositySoftware.RunResult.Entities;
using CuriositySoftware.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.RunResult.Services
{
    public class TestRunService
    {
        private ConnectionProfile m_ConnectionProfile;

        public TestRunService(ConnectionProfile conProfile)
        {
            this.m_ConnectionProfile = conProfile;
        }


        public void PostTestRun(TestPathRunEntity r)
        {
            RestClient client = new RestClient(m_ConnectionProfile.Url);
            client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

            RestRequest request = new RestRequest("/api/apikey/" + m_ConnectionProfile.APIKey + "/model/version/profile/testcollection/testsuite/testpath/run", Method.POST);
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(r);

            try
            {
                IRestResponse response = client.Execute(request);

                if (!response.StatusCode.ToString().Equals("OK"))
                {
                    Console.WriteLine("Failed : HTTP error code - SaveProject : " + response.Content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed : - SaveProject : " + e.Message);
            }
        }
    }
}
