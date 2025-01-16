# OPA Demonstration Solution

## Solution Components


### Infrastructure Components

| Component Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


### Backend Services

| Component Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |
| Mock OAuth Service       | /src/services/oauth | Provides a simple OAuth server. |
| Policy Service        | /src/services/policies  | The service that is responsible for creating and modifying policies for a tenant. |
| Tenant Admin Service       | /src/services/tenant | The service that... |
| Global Admin Service       | /src/services/admin | The service that... |

### Client SDKs

| Component Name | Location | Description |
|----------------|----------|-------------|
| Policy Management Client       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |
| Policy Evaluation Client       | /src/services/oauth | Provides a simple OAuth server. |

### Frontend Applications
| Component Name | Location | Description |
|----------------|----------|-------------|
| Tenant Portal       | /src/frontend/tenant-portal | The portal that tenants can use to directly manage policies, resources, and various other configuration elements. |
| Admin Portal       | /src/frontend/global-admin-portal| The portal that system admins can use to manage tenants as well as all of the resources within a tenant. |

### OPA Policies

| Component Name | Location | Description |
|----------------|----------|-------------|
| Streaming Policy       | /src/frontend/tenant-portal | The portal that tenants can use to directly manage policies, resources, and various other configuration elements. |
| Pull Policy       | /src/frontend/global-admin-portal| The portal that system admins can use to manage tenants as well as all of the resources within a tenant. |
| mb.cmd       | /src/frontend/global-admin-portal| The portal that system admins can use to manage tenants as well as all of the resources within a tenant. |

### Utilities

| Component Name | Location | Description |
|----------------|----------|-------------|
| Data Generator       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


### Load Tests

| Component Name | Location | Description |
|----------------|----------|-------------|
| Policy Evaluation Tests       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |
| Policy Management Tests       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


## Container Inventory

### MongoDB
The MongoDB container provides data storage for tenant and policy information.
| Resource Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


### OPA
OPA provides policy evaluation services for the solution and policies have been provided that run a real-time sync with MongoDB as well as a  pull/polling style policy.

| Resource Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


### Vault
HashiCorp Vault provides secret services and is integrated with OPA to provide security for MongoDB credentials within the pull-style policy.

| Resource Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


### NGINX
NGINX provides services for OPA to download bundles created from source.  Alternatively, you can mount the volume directly, but this is a more representative solution.

| Resource Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |


### OAuth Mock Server
The OAuth Mock Server provides token services for all developed services as well as OPA.

| Resource Name | Location | Description |
|----------------|----------|-------------|
| Docker-Related Components       | /src/docker | Docker compose files and content for services deployed within the compose file, such as configuration files, etc.  |



## Pre-Reqs

https://jmeter.apache.org/download_jmeter.cgi


## Solution Overview

The solution utilizes a MongoDB datastore that serves as the data source for managing tenants as well as managing policy data. Tenants are consuming systems/applications of the service that serve as a **physical** division within the collection, where each tenant-specific record is segmented by the  ```tenantId``` attribute. All consuming services read the ```tenant_id``` claim from the provided JWT and utilize that to enforce the division of data and ensure that no records are read or written from other tenants.


## Solution Notes

This does not provide a fully optimized data structure for integration with OPA which can cause substantial memory usage, particularly above 100,000 policy records. It is advised that subscribers to the emitted policy events shape the data in an appropriate manner for OPA that is oriented towards optimized reads within OPA rather than providing rich data. 







