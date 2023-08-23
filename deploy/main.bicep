param location string = resourceGroup().location
param environmentName string = 'env-${uniqueString(resourceGroup().id)}'

param minReplicas int = 0

param ordersServiceImage string 
param ordersServicePort int = 80
var ordersServiceAppName = 'orders-app'

param storeapiImage string
param storeapiPort int = 80
var storeapiServiceAppName = 'storeapi-app'

param inventoryImage string
param inventoryPort int = 80
var inventoryServiceAppName = 'inventory-app'

param isPrivateRegistry bool = true

param containerRegistry string
param containerRegistryUsername string = 'testUser'
@secure()
param containerRegistryPassword string = ''
param registryPassword string = 'registry-password'


// Container Apps Environment 
module environment 'environment.bicep' = {
  name: '${deployment().name}--environment'
  params: {
    environmentName: environmentName
    location: location
    appInsightsName: '${environmentName}-ai'
    logAnalyticsWorkspaceName: '${environmentName}-la'
  }
}

// Cosmosdb
module cosmosdb 'cosmosdb.bicep' = {
  name: '${deployment().name}--cosmosdb'
  params: {
    location: location
    primaryRegion: location
  }
}

// storeapi App
module storeapiService 'container-http.bicep' = {
  name: '${deployment().name}--${storeapiServiceAppName}'
  dependsOn: [
    environment
  ]
  params: {
    enableIngress: true
    isExternalIngress: true
    location: location
    environmentName: environmentName
    containerAppName: storeapiServiceAppName
    containerImage: storeapiImage
    containerPort: storeapiPort
    isPrivateRegistry: isPrivateRegistry 
    minReplicas: minReplicas
    containerRegistry: containerRegistry
    registryPassword: registryPassword
    containerRegistryUsername: containerRegistryUsername
    revisionMode: 'Multiple'
    env: [
      {
        name: 'ORDERS_SERVICE_NAME'
        value: ordersServiceAppName
      }
      {
        name: 'INVENTORY_SERVICE_NAME'
        value: inventoryServiceAppName
      }
    ]
    secrets: [
      {
        name: registryPassword
        value: containerRegistryPassword
      }
    ]
  }
}

// inventory App
module inventoryService 'container-http.bicep' = {
  name: '${deployment().name}--${inventoryServiceAppName}'
  dependsOn: [
    environment
  ]
  params: {
    enableIngress: true
    isExternalIngress: true
    location: location
    environmentName: environmentName
    containerAppName: inventoryServiceAppName
    containerImage: inventoryImage
    containerPort: inventoryPort
    isPrivateRegistry: isPrivateRegistry 
    minReplicas: minReplicas
    containerRegistry: containerRegistry
    registryPassword: registryPassword
    containerRegistryUsername: containerRegistryUsername
    revisionMode: 'Multiple'
    env: [
      {
        name: 'ORDERS_SERVICE_NAME'
        value: ordersServiceAppName
      }
      {
        name: 'INVENTORY_SERVICE_NAME'
        value: inventoryServiceAppName
      }
    ]
    secrets: [
      {
        name: registryPassword
        value: containerRegistryPassword
      }
    ]
  }
}

resource stateDaprComponent 'Microsoft.App/managedEnvironments/daprComponents@2022-01-01-preview' = {
  name: '${environmentName}/items'
  dependsOn: [
    environment
  ]
  properties: {
    componentType: 'state.azure.cosmosdb'
    version: 'v1'
    secrets: [
      {
        name: 'masterkey'
        value: cosmosdb.outputs.primaryMasterKey
      }
    ]
    metadata: [
      {
        name: 'url'
        value: cosmosdb.outputs.documentEndpoint
      }
      {
        name: 'database'
        value: 'itemsDb'
      }
      {
        name: 'collection'
        value: 'items'
      }
      {
        name: 'masterkey'
        secretRef: 'masterkey'
      }
    ]
    scopes: [
      ordersServiceAppName
    ]
  }
}

// orders App
module ordersService 'container-http.bicep' = {
  name: '${deployment().name}--${ordersServiceAppName}'
  dependsOn: [
    environment
  ]
  params: {
    enableIngress: true 
    isExternalIngress: true
    location: location
    environmentName: environmentName
    containerAppName: ordersServiceAppName
    containerImage: ordersServiceImage
    containerPort: ordersServicePort
    minReplicas: minReplicas
    isPrivateRegistry: isPrivateRegistry 
    containerRegistry: containerRegistry
    registryPassword: registryPassword
    containerRegistryUsername: containerRegistryUsername
    revisionMode: 'Single'
    secrets: [
      {
        name: registryPassword
        value: containerRegistryPassword
      }
    ]
  }
}


output ordersFqdn string = ordersService.outputs.fqdn
output storeapiFqdn string = storeapiService.outputs.fqdn
output inventoryFqdn string = inventoryService.outputs.fqdn
