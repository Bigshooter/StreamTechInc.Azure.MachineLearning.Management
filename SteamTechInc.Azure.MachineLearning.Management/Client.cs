using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    /// <summary>
    /// This client assumes you have setup an application in your Azure AD and granted the appropriate level of privledge so that
    /// the client application id and key can be used to manage your Azure ML resources.
    /// </summary>
    public class Client
    {
        private string _apiVersion;
        private HttpClient _client;
        private string _bearerToken;
        private string _tenantId;
        private string _applicationId;
        private string _applicationKey;
        private string _managementLoginUri = "https://login.microsoftonline.com/{0}/";
        private string _resourceUri = "https://management.azure.com/";

        /// <summary>
        /// Send in enough information that the client can authenticate on behalf of the application and get a bearer token, 
        /// allowing it to communicate with the Azure management interface.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="applicationId"></param>
        /// <param name="applicationKey"></param>
        /// <param name="apiVersion"></param>
        public Client(string tenantId, string applicationId, string applicationKey, string apiVersion)
        {
            _tenantId = tenantId;
            _applicationId = applicationId;
            _applicationKey = applicationKey;
            _apiVersion = apiVersion;
        }

        /// <summary>
        /// Authenticate with Azure AD, getting a bearer token so you can perform any actions against the management interface.
        /// </summary>
        /// <returns></returns>
        public async Task<ClientResponse> Authenticate()
        {
            ClientResponse returnValue = new ClientResponse();
            returnValue.IsSuccess = false;

            using (HttpClient AuthClient = new HttpClient())
            {
                AuthClient.BaseAddress = new Uri(string.Format(_managementLoginUri, _tenantId));
                AuthClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                string body = "grant_type=client_credentials&";
                body += "client_id=" + _applicationId + "&";
                body += "client_secret=" + _applicationKey + "&";
                body += "resource=" + _resourceUri;

                StringContent payload = new StringContent(body,System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage authResponse = await AuthClient.PostAsync("oauth2/token", payload);

                if(authResponse.IsSuccessStatusCode)
                {
                    //store bearer token for use.                   
                    dynamic returnPayload = JsonConvert.DeserializeObject(await authResponse.Content.ReadAsStringAsync());
                    if(returnPayload!=null)
                    {
                        if(returnPayload.token_type == "Bearer")
                        {
                            _bearerToken = returnPayload.access_token;
                            CreateClient();
                            returnValue.IsSuccess = true;
                        }

                        
                    }
                     
                }
                else
                {
                    returnValue.ResponseMessage = await authResponse.Content.ReadAsStringAsync();
                    returnValue.IsSuccess = false;
                }

            }

            return returnValue;
        }

        private void CreateClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_resourceUri);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        /// <summary>
        /// Attempts to create a webservice
        /// </summary>
        /// <param name="webserviceName"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="webService"></param>
        /// <returns></returns>
        public async Task<ClientResponse> CreateUpdateWebService(string webserviceName, string subscriptionId, string resourceGroupName, WebService webService)
        {
            ClientResponse returnValue = new ClientResponse();
            returnValue.IsSuccess = false;

            string putUrl = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.MachineLearning/webServices/{2}?api-version={3}", subscriptionId, resourceGroupName, webserviceName, _apiVersion);

            string json = JsonConvert.SerializeObject(webService);
            StringContent payload = new StringContent(json,System.Text.Encoding.UTF8,"application/json");


            HttpResponseMessage response = await _client.PutAsync(putUrl, payload);
            if(response.IsSuccessStatusCode)
            {
                returnValue.IsSuccess = true;
            }
            else
            {
                returnValue.ResponseMessage = await response.Content.ReadAsStringAsync();
                returnValue.IsSuccess = false;
            }

            return returnValue;

        }

    }
}
