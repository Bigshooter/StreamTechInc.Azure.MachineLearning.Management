# StreamTechInc.Azure.MachineLearning.Management
This Rest Client will allow you to retrain existing web service and use those iLearners to provision new web services.

Example


Client newManagementClient = new Client(tenantId, applicationId, applicaitonKey, apiVersion);
ClientResponse response = await newManagementClient.Authenticate();
if(response.IsSuccess)
{
    //authentication successful, do some work
    response = await newManagementClient.GetWebService("webservicename", subscriptionId, resourceGroupName);
}
else
{

}

