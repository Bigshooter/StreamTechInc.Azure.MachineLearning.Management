# StreamTechInc.Azure.MachineLearning.Management
This Rest Client will allow you to retrain existing web service and use those iLearners to provision new web services.

To use this you will need most if not all of the following:
    * Tenant Id - 
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
Client newManagementClient = new Client(tenantId, applicationId, applicaitonKey, apiVersion);
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