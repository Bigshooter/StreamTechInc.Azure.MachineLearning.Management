# StreamTechInc.Azure.MachineLearning.Management

This Rest Client will allow an authorized application to retrain existing Azure Machine Learning web services and use those iLearners that are created to provision new web services.
To do this you first go into Azure Machine Learning UI and create a retrained web service and a predictive web service.  Now the training web service can be used as a service to produce iLearners
and the predictive webservice can be used as a scafold for making new services with new ilearners.

To use this you will need most if not all of the following:
  * Tenant Id - Azure AD id.
  * Application Id - the id Azure gave your application when adding it to Azure Ad.
  * Application Key - the key you created in the Azure portal when adding an application to Azure AD.
  * Api Version - the version of the machine learning API you are using.
  * Subscription Id - your azure Subscription Id.
  * Resource Group Name - Azure resource group holding your Machine learning workspace.
  * Machine Learning Workspace Id - Check the properties pane of the Machine Learning Workspace in Azure portal.
  * Machine Learning Workspace Plan Id - So far as I can tell this is the name of your machine learning workspace.
  * Azure Storage Account Name - The storage account name where you store your machine learning blobs.
  * Azure Storage Account Key - The storage account key where you store your machine learning blobs.

Example

``` c#
     

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

        response = newManagementClient.CreateUpdateWebService("HelloWorld", subscriptionId, resourceGroupName, predictiveWebService).Result;
        if(response.IsSuccess)
        {
            WebService newPredictiveWebService = JsonConvert.DeserializeObject<WebService>(response.ResponseMessage);
            if(newPredictiveWebService.Properties.ProvisioningState == "Succeeded")
            {
                //all good

            }
            else if(newPredictiveWebService.Properties.ProvisioningState == "Failed")
            {
                //add bad
            }
            else
            {
                //unknown or provisioning
                //should be a loop
                Thread.Sleep(5000);
                response = newManagementClient.GetWebService("HelloWorld", subscriptionId, resourceGroupName).Result;
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

```