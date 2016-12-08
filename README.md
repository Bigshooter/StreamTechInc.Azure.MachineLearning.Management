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
ClientResponse response = await newManagementClient.Authenticate();
if(response.IsSuccess)
{
    //authentication successful, do some work
    response = await newManagementClient.GetWebService("webservicename", subscriptionId, resourceGroupName);
    if(response.IsSuccess)
    {
        WebService myService = JsonConvert.DeserializeObject<WebService>(response.ResponseMessage);
    }
    else
    {

    }

}
else
{

}

```