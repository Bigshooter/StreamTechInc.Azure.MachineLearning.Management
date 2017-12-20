using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    /// <summary>
    /// This client assumes you have setup an application in your Azure AD and granted the appropriate level of privledge so that
    /// the client application id and key can be used to manage your Azure ML resources.
    /// https://docs.microsoft.com/en-us/rest/api/index?redirectedfrom=MSDN
    /// https://docs.microsoft.com/en-gb/azure/azure-resource-manager/resource-group-create-service-principal-portal
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
        private DateTime _authExiryDateTime;

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
        /// Authenticate with Azure AD, getting a bearer token so you can perform any actions against the management interface.  If unsuccessful check the returned message.
        /// </summary>
        /// <returns></returns>
        public async Task<ClientResponse> Authenticate()
        {
            //https://docs.microsoft.com/en-gb/azure/active-directory/active-directory-protocols-oauth-service-to-service#request-an-access-token

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

                StringContent payload = new StringContent(body, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage authResponse = await AuthClient.PostAsync("oauth2/token", payload);

                if (authResponse.IsSuccessStatusCode)
                {
                    //store bearer token for use.                   
                    dynamic returnPayload = JsonConvert.DeserializeObject(await authResponse.Content.ReadAsStringAsync());
                    if (returnPayload != null)
                    {
                        if (returnPayload.token_type == "Bearer")
                        {
                            _bearerToken = returnPayload.access_token;
                            CreateClient();
                            //capture expiry time
                            long exiryTimeInSeconds = long.Parse(returnPayload.expires_on.Value);
                            DateTimeConvert dateTimeConvert = new DateTimeConvert();
                            dateTimeConvert.SetTime(exiryTimeInSeconds);
                            _authExiryDateTime = dateTimeConvert.GetDateTime();
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
        /// Creates a webservice.  If unsuccessful the return message has the reason for the failure contained within it.
        /// </summary>
        /// <param name="webserviceName"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="webService"></param>
        /// <returns></returns>
        public async Task<ClientResponse> CreateUpdateWebService(string webserviceName, string subscriptionId, string resourceGroupName, WebService webService)
        {
            ClientResponse clientResponse = new ClientResponse();
            clientResponse.IsSuccess = false;

            try
            {
                //reauth if our bearer token has expired
                if (_authExiryDateTime.AddMinutes(-2) <= DateTime.UtcNow)
                {
                    await Authenticate();
                }

                string putUrl = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.MachineLearning/webServices/{2}?api-version={3}", subscriptionId, resourceGroupName, webserviceName, _apiVersion);

                string json = JsonConvert.SerializeObject(webService, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                //remove values that cause the azure ML service to have a fit
                JObject remover = JsonConvert.DeserializeObject<JObject>(json);
                remover.Remove("id");
                remover.Remove("type");
                JToken properties = remover.Children<JProperty>().First(x => x.Name == "properties").First();
                properties.Children<JProperty>().First(x => x.Name == "createdOn").Remove();
                //properties.Children<JProperty>().First(x => x.Name == "keys").Remove();
                properties.Children<JProperty>().First(x => x.Name == "modifiedOn").Remove();
                properties.Children<JProperty>().First(x => x.Name == "provisioningState").Remove();
                properties.Children<JProperty>().First(x => x.Name == "swaggerLocation").Remove();
                properties.Children<JProperty>().First(x => x.Name == "diagnostics").First().Children<JProperty>().First(z => z.Name == "expiry").Remove();
                properties.Children<JProperty>().First(x => x.Name == "exampleRequest").First().Children<JProperty>().First(z => z.Name == "globalParameters").Remove();

                json = JsonConvert.SerializeObject(remover);

                StringContent payload = new StringContent(json, System.Text.Encoding.UTF8, "application/json");


                HttpResponseMessage response = await _client.PutAsync(putUrl, payload);
                if (response.IsSuccessStatusCode)
                {
                    clientResponse.IsSuccess = true;
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                clientResponse.ResponseMessage = ex.ToString();
            }

            return clientResponse;

        }


        /// <summary>
        /// Gets the definition of an existing web service.  If successful the response message has the json in it.  Otherwise it has the reason for the failure.
        /// </summary>
        /// <param name="webserviceName">name of the service you want to get</param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <returns></returns>
        public async Task<ClientResponse> GetWebService(string webserviceName, string subscriptionId, string resourceGroupName)
        {
            ClientResponse clientResponse = new ClientResponse();
            clientResponse.IsSuccess = false;

            try
            {
                //reauth if our bearer token has expired
                if (_authExiryDateTime.AddMinutes(-2) <= DateTime.UtcNow)
                {
                    await Authenticate();
                }

                //subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearning/webServices/{webServiceName}?api-version=2016-05-01-preview
                string getUrl = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.MachineLearning/webServices/{2}?api-version={3}", subscriptionId, resourceGroupName, webserviceName, _apiVersion);

                HttpResponseMessage response = await _client.GetAsync(getUrl);
                if (response.IsSuccessStatusCode)
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = true;
                }
                else
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                clientResponse.ResponseMessage = ex.ToString();
            }

            return clientResponse;
        }


        /// <summary>
        /// Gets the keys created to communicate with the new webservice.
        /// </summary>
        /// <param name="webserviceName"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="resourceGroupName"></param>
        /// <returns></returns>
        public async Task<ClientResponse> GetWebServiceKeys(string webserviceName, string subscriptionId, string resourceGroupName)
        {
            ClientResponse clientResponse = new ClientResponse();
            clientResponse.IsSuccess = false;

            try
            {
                //reauth if our bearer token has expired
                if (_authExiryDateTime.AddMinutes(-2) <= DateTime.UtcNow)
                {
                    await Authenticate();
                }

                ///subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearning/webServices/{webServiceName}/listKeys?api-version=2016-05-01-preview
                string getUrl = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.MachineLearning/webServices/{2}/listKeys?api-version={3}", subscriptionId, resourceGroupName, webserviceName, _apiVersion);

                HttpResponseMessage response = await _client.GetAsync(getUrl);
                if (response.IsSuccessStatusCode)
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = true;
                }
                else
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                clientResponse.ResponseMessage = ex.ToString();
            }

            return clientResponse;
        }


        public async Task<ClientResponse> DeleteWebService(string webserviceName, string subscriptionId, string resourceGroupName)
        {

            ClientResponse clientResponse = new ClientResponse();
            clientResponse.IsSuccess = false;

            try
            {
                //reauth if our bearer token has expired
                if (_authExiryDateTime.AddMinutes(-2) <= DateTime.UtcNow)
                {
                    await Authenticate();
                }

                ///subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.MachineLearning/webServices/{webServiceName}?api-version=2016-05-01-preview
                string deleteUrl = string.Format("subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.MachineLearning/webServices/{2}?api-version={3}", subscriptionId, resourceGroupName, webserviceName, _apiVersion);

                HttpResponseMessage response = await _client.DeleteAsync(deleteUrl);
                if (response.IsSuccessStatusCode)
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = true;
                }
                else
                {
                    clientResponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    clientResponse.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                clientResponse.ResponseMessage = ex.ToString();
            }

            return clientResponse;
        }

    }
}
