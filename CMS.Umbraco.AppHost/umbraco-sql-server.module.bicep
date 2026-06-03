@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource sqlServerAdminManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' = {
  name: take('umbraco_sql_server-admin-${uniqueString(resourceGroup().id)}', 63)
  location: location
}

resource umbraco_sql_server 'Microsoft.Sql/servers@2023-08-01' = {
  name: take('umbracosqlserver-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      login: sqlServerAdminManagedIdentity.name
      sid: sqlServerAdminManagedIdentity.properties.principalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    version: '12.0'
  }
  tags: {
    'aspire-resource-name': 'umbraco-sql-server'
  }
}

resource sqlFirewallRule_AllowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2023-08-01' = {
  name: 'AllowAllAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
  parent: umbraco_sql_server
}

resource UmbracoDb 'Microsoft.Sql/servers/databases@2023-08-01' = {
  name: 'UmbracoDb'
  location: location
  properties: {
    freeLimitExhaustionBehavior: 'AutoPause'
    useFreeLimit: true
  }
  sku: {
    name: 'GP_S_Gen5_2'
  }
  parent: umbraco_sql_server
}

output sqlServerFqdn string = umbraco_sql_server.properties.fullyQualifiedDomainName

output name string = umbraco_sql_server.name

output id string = umbraco_sql_server.id

output sqlServerAdminName string = umbraco_sql_server.properties.administrators.login