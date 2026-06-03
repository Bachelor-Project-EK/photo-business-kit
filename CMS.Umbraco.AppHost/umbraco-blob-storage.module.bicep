@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource umbraco_blob_storage 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('umbracoblobstorage${uniqueString(resourceGroup().id)}', 24)
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    accessTier: 'Hot'
    allowSharedKeyAccess: false
    isHnsEnabled: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: {
    'aspire-resource-name': 'umbraco-blob-storage'
  }
}

output blobEndpoint string = umbraco_blob_storage.properties.primaryEndpoints.blob

output dataLakeEndpoint string = umbraco_blob_storage.properties.primaryEndpoints.dfs

output queueEndpoint string = umbraco_blob_storage.properties.primaryEndpoints.queue

output tableEndpoint string = umbraco_blob_storage.properties.primaryEndpoints.table

output name string = umbraco_blob_storage.name

output id string = umbraco_blob_storage.id