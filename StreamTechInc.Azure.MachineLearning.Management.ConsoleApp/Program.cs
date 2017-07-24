using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamTechInc.Azure.MachineLearning.Management;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Serialization;

namespace StreamTechInc.Azure.MachineLearning.Management.ConsoleApp
{
    //https://docs.microsoft.com/en-us/rest/api/machinelearning/webservices#WebServices_CreateOrUpdate
    public class Program
    {
        public static void Main(string[] args)
        {

            string tenantId = "";
            string applicationId = "";
            string applicationKey = "";
            string apiVersion = "";
            string subscriptionId = "";
            string resourceGroupName = "";
            string workspaceId = "";
            string webservicePlanId = "";
            string storageAccountName = "";
            string storageAccountKey = "";
            

            Client newManagementClient = new Client(tenantId, applicationId, applicationKey, apiVersion);

            if(newManagementClient.Authenticate().Result.IsSuccess)
            {

                string predictiveWebServiceScafold = "MultiClassRetrai.2016.12.7.19.47.0.349";

                ClientResponse response = newManagementClient.GetWebService(predictiveWebServiceScafold, subscriptionId, resourceGroupName).Result;
                if(response.IsSuccess)
                {
                    //this is temporary until I figure out what is wrong with json serialization
                    List<string> problems = new List<string> { "Name","Type","LocationInfo","Uri","Credentials",
                                                                "Title","Description","Properties","Format","OutputPorts",
                                                                "Edges", "GlobalParameters", "Inputs","GraphParameters","Nodes",
                                                               "SourceNodeId","SourcePortId","TargetNodeId","TargetPortId","AssetId",
                                                               "Parameters","InputId", "OutputId"};
                    string temp = response.ResponseMessage;
                    foreach (string problem in problems)
                    {
                        char fistLetter = problem[0];
                        string replacementString = fistLetter.ToString().ToLower() + problem.Substring(1, problem.Length - 1);
                        temp = temp.Replace(problem, replacementString);
                    }
                    temp = temp.Replace("graphparameters", "graphParameters");

                    WebService predictiveWebService = JsonConvert.DeserializeObject<WebService>(temp, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    predictiveWebService.Name = "HelloWorld";
                    predictiveWebService.Id = "";
                    predictiveWebService.Properties.Title = "HelloWorld";
                    predictiveWebService.Properties.StorageAccount.Key = storageAccountKey;
                    predictiveWebService.Properties.CommitmentPlan = new CommitmentPlan();
                    predictiveWebService.Properties.CommitmentPlan.Id = webservicePlanId;
                    predictiveWebService.Properties.MachineLearningWorkspace = new MachineLearningWorkspace();
                    predictiveWebService.Properties.MachineLearningWorkspace.Id = workspaceId;
                    //remove provisioning state, the service wont allow it.
                    predictiveWebService.Properties.ProvisioningState = "";
                    predictiveWebService.Properties.SwaggerLocation = "";
                    predictiveWebService.Type = "";
                    predictiveWebService.Properties.Keys = new WebServiceKeys();
                    predictiveWebService.Properties.Keys.Primary = "iI1aSqFrqsU1T6lkrGflvoMi2V45BMDSLspcFYqkt31qjEympwe7WhDXLlt+LoCpBIxFZvDWAbqdnW7+SPcySA==";
                    predictiveWebService.Properties.Keys.Secondary = "eI1aSqFrqsU1T6lkrGflvoMi2V45BMDSLspcFYqkt31qjEympwe7WhDXLlt+LoCpBIxFZvDWAbqdnW7+SPcySA==";


                    response = newManagementClient.CreateUpdateWebService("HelloWorld", subscriptionId, resourceGroupName, predictiveWebService).Result;
                    if(response.IsSuccess)
                    {
                        WebService newPredictiveWebService = JsonConvert.DeserializeObject<WebService>(response.ResponseMessage);
                        if(newPredictiveWebService.Properties.ProvisioningState == "Succeeded")
                        {
                            //all good
                            //do a getkeys on the new webservice
                            //update the solution with the batch URL & key
                        }
                        else if(newPredictiveWebService.Properties.ProvisioningState == "Failed")
                        {
                            //add bad
                            //fail the queue operation
                        }
                        else
                        {
                            //unknown or provisioning
                            //should be a loop
                            Thread.Sleep(15000);
                            response = newManagementClient.GetWebServiceKeys("HelloWorld", subscriptionId, resourceGroupName).Result;
                            if(response.IsSuccess)
                            {
                                WebService newPredictiveWebService2 = JsonConvert.DeserializeObject<WebService>(response.ResponseMessage);
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.ResponseMessage);
                    }


                }
                else
                {

                }

            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
