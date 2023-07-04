
using CuriositySoftware.Utils;
using RestSharp;
using System;
using TestModeller_CSharp.DataAllocation.Entities;

namespace TestModeller_CSharp.DataAllocation.Services
{
    public class DataCriteriaService {
        ConnectionProfile ConnectionProfile;

        String ErrorMessage;

        public DataCriteriaService(ConnectionProfile connectionProfile)
        {
            ConnectionProfile = connectionProfile;
        }

        public DataCatalogueTestCriteria GetTestCriteria(String catalogueName, String criteriaName)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);
                client.AddHandler(contentType: "application/json", deserializer: NewtonsoftJsonSerializer.Default);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/data-catalogue/" + catalogueName + "/test-criteria/" + criteriaName, Method.GET);
                request.JsonSerializer = NewtonsoftJsonSerializer.Default;
                request.RequestFormat = DataFormat.Json;


                IRestResponse< DataCatalogueTestCriteria> response = client.Execute< DataCatalogueTestCriteria>(request);

                if (response.StatusCode.Equals("OK"))
                {
                    return response.Data;
                } else {
                    ErrorMessage = "Failed : HTTP error code - GetTestCriteria : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return null;
                }
            } catch (Exception e) {
                ErrorMessage = "Failed : - GetTestCriteria : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return null;
            }
        }

        public String getErrorMessage() {
            return ErrorMessage;
        }
    }
}