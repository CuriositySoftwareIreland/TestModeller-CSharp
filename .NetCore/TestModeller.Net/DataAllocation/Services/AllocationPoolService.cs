
using CuriositySoftware.Utils;
using RestSharp;
using System;
using TestModeller_CSharp.DataAllocation.Entities;

namespace TestModeller_CSharp.DataAllocation.Services
{
    public class AllocationPoolService {

        ConnectionProfile ConnectionProfile;

        String ErrorMessage;

        public AllocationPoolService(ConnectionProfile connectionProfile)
        {
            ConnectionProfile = connectionProfile;
        }

        public AllocationPool CreateAllocationPool(AllocationPool allocationPool)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(allocationPool);

                RestResponse<AllocationPool> response = client.Execute<AllocationPool>(request);

                if (response.StatusCode.Equals("OK")) {
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

        public AllocationPool GetAllocationPool(String name)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/name/" + name, Method.Get);
                request.RequestFormat = DataFormat.Json;


                RestResponse<AllocationPool> response = client.Execute<AllocationPool>(request);

                if (response.StatusCode.Equals("OK"))
                {
                    return response.Data;
                } else {
                    ErrorMessage = "Failed : HTTP error code - GetAllocationPool : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return null;
                }
            } catch (Exception e) {
                ErrorMessage = "Failed : - GetAllocationPool : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return null;
            }
        }

        public Boolean DeleteAllocationPool(long? id)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/allocation-pool/name/" + id, Method.Delete);
                request.RequestFormat = DataFormat.Json;


                RestResponse response = client.Execute(request);

                if (response.StatusCode.Equals("OK")) { 
                    return true;
                } else {
                    ErrorMessage = "Failed : HTTP error code - GetAllocationPool : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return false;
                }
            } catch (Exception e) {
                ErrorMessage = "Failed : - DeleteAllocationPool : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return false;
            }
        }

        public String getErrorMessage() {
            return ErrorMessage;
        }
    }
}