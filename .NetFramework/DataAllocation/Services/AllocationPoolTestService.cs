using CuriositySoftware.Utils;
using RestSharp;
using System;
using TestModeller_CSharp.DataAllocation.Entities;

namespace TestModeller_CSharp.DataAllocation.Services
{
    public class AllocationPoolTestService {
        ConnectionProfile ConnectionProfile;

        String ErrorMessage;

        public AllocationPoolTestService(ConnectionProfile connectionProfile)
        {
            ConnectionProfile = connectionProfile;
        }

        public AllocatedTest CreateAllocatedTest(AllocatedTest allocatedTest, long? poolId)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);
                client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/" + poolId + "/allocated-test", Method.POST);
                request.JsonSerializer = NewtonsoftJsonSerializer.Default;
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(allocatedTest);

                IRestResponse<AllocatedTest> response = client.Execute<AllocatedTest>(request);

                if (response.StatusCode.Equals("OK"))
                {
                    return response.Data;
                } else {
                    ErrorMessage = "Failed : HTTP error code - CreateAllocationPool : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return null;
                }
            } catch (Exception e) {
                ErrorMessage = "Failed : - CreateAllocationPool : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return null;
            }
        }

        public Boolean DeleteAllocationPoolTest(long? id)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);
                client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/allocated-test/" + id, Method.DELETE);
                request.JsonSerializer = NewtonsoftJsonSerializer.Default;
                request.RequestFormat = DataFormat.Json;

                IRestResponse<AllocatedTest> response = client.Execute<AllocatedTest>(request);

                if (response.StatusCode.Equals("OK")) { 
                    return true;
                } else {
                    ErrorMessage = "Failed : HTTP error code - DeleteAllocationPoolTest : " + response.Content;

                    return false;
                }
            } catch (Exception e) {
                ErrorMessage = "Failed : - DeleteAllocationPoolTest : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return false;
            }

        }

        public String getErrorMessage() {
            return ErrorMessage;
        }

    }
}