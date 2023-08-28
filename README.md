# Container Apps Store Microservice Sample

Based on: https://github.com/Azure-Samples/container-apps-store-api-microservice .

This repository was created to help users deploy a microservice-based sample application (rewritten by repository author to .NET) to Azure Container Apps. 

Azure Container Apps is a managed serverless container offering for building and deploying modern apps at scale. It enables developers to deploy containerized apps without managing container orchestration. This sample makes use of the Distributed Application Runtime (Dapr), which is integrated deeply into the container apps platform.

Dapr is a CNCF project that helps developers overcome the inherent challenges presented by distributed applications, such as state management and service invocation. Container Apps also provides a fully-managed integration with the Kubernetes Event Driven Autoscaler (KEDA). KEDA allows your containers to autoscale based on incoming events from external services such Azure Service Bus or Redis.

## Solution Overview

![image of architecture](./assets/arch.png)

There are three main microservices in the solution.

#### Store API (`storeapi-app`)

The [`storeapi-app`](./StoreAPI) is a .NET 6 ASP .NET Core API app that exposes some endpoints. `/swagger` will return the swagger described api page, `/FetchOrder` will return details on an order (retrieved from the **order service**), `/CreateOrder` will crete order (through **order service**, and `/ListInventory` will return details on inventory items (retrieved from the **inventory service**).

#### Order Service (`orders-app`)

The [`orders-app`](./OrdersService) is a .NET 6 ASP .NET Core API app that will retrieve and store the state of orders. It uses [Dapr state management](https://docs.dapr.io/developing-applications/building-blocks/state-management/state-management-overview/) to store the state of the orders. When deployed in Container Apps, Dapr is configured to point to an Azure Cosmos DB to back the state.

#### Inventory Service (`inventory-app`)

The [`inventory-app`](./go-service) is a .NET 6 ASP .NET Core API app that will retrieve and store the state of inventory. For this sample, the app just returns back a static value.

## Deploy via GitHub Actions

> **IMPORTANT NOTE**: This tutorial has been updated (8/2022) to use GITHUB.TOKEN instead of a GH PAT (Personal Access Token). If you have run this tutorial already, and have images that were pushed using a PAT, you will need to delete these from GHCR for the workflow to successfully write the updated images.

The entire solution is configured with [GitHub Actions](https://github.com/features/actions) and [Bicep](https://docs.microsoft.com/azure/azure-resource-manager/bicep/overview) for CI/CD
1. Fork the sample repo
2. Create the following required [encrypted secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets#creating-encrypted-secrets-for-a-repository) for the sample

| Name              | Value                                                                                                                                                                                                                                                                                                                                                                                                  |
| ----------------- |--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AZURE_CREDENTIALS | The JSON credentials for an Azure subscription. Make sure the Service Principal has permissions at the subscription level scope [Learn more](https://docs.microsoft.com/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret). Use the option in this article: `Use the Azure login action with a service principal secret.` |
| RESOURCE_GROUP | The name of the resource group to create                                                                                                                                                                                                                                                                                                                                                               |

3. Open GitHub Actions, select the **Build and Deploy** action and choose to run the workflow. 

   The GitHub action performs the following actions:
    - Create Resource Group and Azure Container Registry for your containers.
    - Build the code and container image for each microservice
    - Push the images to your crated Azure Container Registry
    - Create an Azure Container Apps environment with an associated Log Analytics workspace and App Insights instance for Dapr distributed tracing
    - Create a Cosmos DB database and associated Dapr component for using Cosmos DB as a state store
    - Deploy Container Apps for each of the microservices

4. Once the GitHub Actions have completed successfully, navigate to the [Azure Portal](https://portal.azure.com) and select the resource group you created. Open the `storeapi-app` container, and browse to the URL. You should be redirected to swagger page where you can test the api nd see the  application running. You can go through the UX to create an order through the order microservice, fetch it or list inventory through provided endpoints and swagger.

5. After calling each microservice, you can open the application insights resource created and select the **Application Map**, you should see a visualization of your calls between Container Apps (note: it may take a few minutes for the app insights data to ingest and process into the app map view).

## Build and Run locally
Not supported. You can run each app locally using IIS or Docker, but calling any endpoint will return an error.


