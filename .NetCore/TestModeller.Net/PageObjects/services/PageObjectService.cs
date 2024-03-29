using CuriositySoftware.PageObjects.Entities;
using CuriositySoftware.Utils;
using RestSharp;
using System;

namespace CuriositySoftware.PageObjects.Services
{ 
    public class PageObjectService {
        private ConnectionProfile ConnectionProfile;

        private String ErrorMessage;

        public PageObjectService(ConnectionProfile connectionProfile)
        {
            this.ConnectionProfile = connectionProfile;

            this.ErrorMessage = "";
        }

        public String GetErrorMessage()
        {
            return this.ErrorMessage;
        }

        public Boolean AddPageObjectHistory(PageObjectHistoryEntity pageObjectHistoryEntity)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/page-collection/page-object/page-object-history", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(pageObjectHistoryEntity);


                RestResponse response = client.Execute(request);

                if (response.StatusCode.Equals("OK")) {
                    return true;
                } else {
                    ErrorMessage = "Failed : HTTP error code - pageObjectHistoryEntity : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return false;
                }
            } catch (Exception e) {
                ErrorMessage = "Failed : - pageObjectHistoryEntity : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return false;
            }
        }

        public Boolean UpdatePageObjectParameter(PageObjectParameterEntity pageObjectParameter)
        {
            try {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/page-collection/page-object/page-object-param", Method.Put);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(pageObjectParameter);


                RestResponse response = client.Execute(request);
                if (response.StatusCode.Equals("OK"))
                {
                    return true;
                }
                else
                {
                    ErrorMessage = "Failed : HTTP error code - UpdatePageObjectParameter : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = "Failed : - UpdatePageObjectParameter : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return false;
            }
        }

        public Boolean UpdatePageObject(PageObjectEntity pageObjectEntity)
        {
            try
            {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/page-collection/page-object", Method.Put);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(pageObjectEntity);


                RestResponse response = client.Execute(request);
                if (response.StatusCode.Equals("OK"))
                {
                    return true;
                }
                else
                {
                    ErrorMessage = "Failed : HTTP error code - UpdatePageObject : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return false;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = "Failed : - UpdatePageObject : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return false;
            }
        }

        public PageObjectEntity GetPageObject(long pageId)
        {
            try
            {
                RestClient client = new RestClient(this.ConnectionProfile.Url);

                RestRequest request = new RestRequest("/api/apikey/" + ConnectionProfile.APIKey + "/page-collection/page-object/" + pageId, Method.Get);
                request.RequestFormat = DataFormat.Json;


                RestResponse<PageObjectEntity> response = client.Execute<PageObjectEntity>(request);
                if (response.StatusCode.Equals("OK"))
                {
                    return response.Data;

                }
                else
                {
                    ErrorMessage = "Failed : HTTP error code - GetPageObject : " + response.Content;

                    Console.WriteLine(ErrorMessage);

                    return null;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = "Failed : - GetPageObject : " + e.Message;

                Console.WriteLine(ErrorMessage);

                return null;
            }
        }
    }
}