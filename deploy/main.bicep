param location string = resourceGroup().location
param environmentName string = 'env-${uniqueString(resourceGroup().id)}'

param minReplicas int = 0

param stateServiceImage string 
param stateServicePort int = 80
var stateServiceAppName = 'state-app'

param svcsvcImage string
param svcsvcPort int = 80
var svcsvcServiceAppName = 'svcsvc-app'

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

// svcsvc App
module svcsvcService 'container-http.bicep' = {
  name: '${deployment().name}--${svcsvcServiceAppName}'
  dependsOn: [
    environment
  ]
  params: {
    enableIngress: true
    isExternalIngress: true
    location: location
    environmentName: environmentName
    containerAppName: svcsvcServiceAppName
    containerImage: svcsvcImage
    containerPort: svcsvcPort
    isPrivateRegistry: isPrivateRegistry 
    minReplicas: minReplicas
    containerRegistry: containerRegistry
    registryPassword: registryPassword
    containerRegistryUsername: containerRegistryUsername
    revisionMode: 'Multiple'
    env: [
      {
        name: 'STATE_SERVICE_NAME'
        value: stateServiceAppName
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
      stateServiceAppName
    ]
  }
}

// state App
module stateService 'container-http.bicep' = {
  name: '${deployment().name}--${stateServiceAppName}'
  dependsOn: [
    environment
  ]
  params: {
    enableIngress: true 
    isExternalIngress: true
    location: location
    environmentName: environmentName
    containerAppName: stateServiceAppName
    containerImage: stateServiceImage
    containerPort: stateServicePort
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


output stateFqdn string = stateService.outputs.fqdn
output svcsvcFqdn string = svcsvcService.outputs.fqdn
