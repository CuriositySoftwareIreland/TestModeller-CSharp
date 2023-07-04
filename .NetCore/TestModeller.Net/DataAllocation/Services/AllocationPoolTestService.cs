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

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/" + poolId + "/allocated-test", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(allocatedTest);

                RestResponse<AllocatedTest> response = client.Execute<AllocatedTest>(request);

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

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/allocated-test/" + id, Method.Delete);
                request.RequestFormat = DataFormat.Json;

                RestResponse<AllocatedTest> response = client.Execute<AllocatedTest>(request);

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