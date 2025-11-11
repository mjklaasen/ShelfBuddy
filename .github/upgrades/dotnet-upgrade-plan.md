# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src\ShelfBuddy.SharedKernel\ShelfBuddy.SharedKernel.csproj
4. Upgrade src\ShelfBuddy.API.Common\ShelfBuddy.API.Common.csproj
5. Upgrade src\ShelfBuddy.Contracts\ShelfBuddy.Contracts.csproj
6. Upgrade src\Inventory\ShelfBuddy.InventoryManagement.Domain\ShelfBuddy.InventoryManagement.Domain.csproj
7. Upgrade src\Inventory\ShelfBuddy.InventoryManagement.Application\ShelfBuddy.InventoryManagement.Application.csproj
8. Upgrade src\Inventory\ShelfBuddy.InventoryManagement.Infrastructure\ShelfBuddy.InventoryManagement.Infrastructure.csproj
9. Upgrade src\ShelfBuddy.ServiceDefaults\ShelfBuddy.ServiceDefaults.csproj
10. Upgrade src\Client\ShelfBuddy.ClientInterface.LocalDb\ShelfBuddy.ClientInterface.LocalDb.csproj
11. Upgrade src\ShelfBuddy.API\ShelfBuddy.API.csproj
12. Upgrade src\Client\ShelfBuddy.ClientInterface.MigrationManager\ShelfBuddy.ClientInterface.MigrationManager.csproj
13. Upgrade src\Client\ShelfBuddy.ClientInterface\ShelfBuddy.ClientInterface.csproj
14. Upgrade src\ShelfBuddy.AppHost\ShelfBuddy.AppHost.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                   | Current Version | New Version      | Description                                   |
|:-----------------------------------------------|:---------------:|:----------------:|:----------------------------------------------|
| Aspire.Hosting.AppHost                         | 9.5.1           | 13.0.0           | Recommended for .NET 10.0                     |
| Aspire.Hosting.Azure.Sql                       | 9.5.1           | 13.0.0           | Recommended for .NET 10.0                     |
| Aspire.Microsoft.EntityFrameworkCore.SqlServer | 9.5.1           | 13.0.0           | Recommended for .NET 10.0                     |
| Microsoft.AspNetCore.OpenApi                   | 9.0.10          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.Design           | 9.0.10          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.Sqlite           | 9.0.10          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting                   | 9.0.10          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Http                      | 9.0.10          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Http.Resilience           | 9.10.0          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Logging.Debug             | 9.0.10          | 10.0.0           | Recommended for .NET 10.0                     |
| Microsoft.Extensions.ServiceDiscovery          | 9.5.1           | 10.0.0           | Recommended for .NET 10.0                     |
| OpenTelemetry.Instrumentation.AspNetCore       | 1.12.0          | 1.14.0-rc.1      | Recommended for .NET 10.0                     |
| OpenTelemetry.Instrumentation.Http             | 1.12.0          | 1.14.0-rc.1      | Recommended for .NET 10.0                     |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### src\ShelfBuddy.SharedKernel\ShelfBuddy.SharedKernel.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### src\ShelfBuddy.API.Common\ShelfBuddy.API.Common.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### src\ShelfBuddy.Contracts\ShelfBuddy.Contracts.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### src\Inventory\ShelfBuddy.InventoryManagement.Domain\ShelfBuddy.InventoryManagement.Domain.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### src\Inventory\ShelfBuddy.InventoryManagement.Application\ShelfBuddy.InventoryManagement.Application.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### src\Inventory\ShelfBuddy.InventoryManagement.Infrastructure\ShelfBuddy.InventoryManagement.Infrastructure.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Aspire.Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.5.1` to `13.0.0` (*recommended for .NET 10.0*)

#### src\ShelfBuddy.ServiceDefaults\ShelfBuddy.ServiceDefaults.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Http.Resilience should be updated from `9.10.0` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.ServiceDiscovery should be updated from `9.5.1` to `10.0.0` (*recommended for .NET 10.0*)
  - OpenTelemetry.Instrumentation.AspNetCore should be updated from `1.12.0` to `1.14.0-rc.1` (*recommended for .NET 10.0*)
  - OpenTelemetry.Instrumentation.Http should be updated from `1.12.0` to `1.14.0-rc.1` (*recommended for .NET 10.0*)

#### src\Client\ShelfBuddy.ClientInterface.LocalDb\ShelfBuddy.ClientInterface.LocalDb.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Sqlite should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)

#### src\ShelfBuddy.API\ShelfBuddy.API.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.OpenApi should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)

#### src\Client\ShelfBuddy.ClientInterface.MigrationManager\ShelfBuddy.ClientInterface.MigrationManager.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)

#### src\Client\ShelfBuddy.ClientInterface\ShelfBuddy.ClientInterface.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0` to `net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0;net10.0-windows`

NuGet packages changes:
  - Microsoft.Extensions.Http should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging.Debug should be updated from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)

#### src\ShelfBuddy.AppHost\ShelfBuddy.AppHost.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Aspire.Hosting.AppHost should be updated from `9.5.1` to `13.0.0` (*recommended for .NET 10.0*)
  - Aspire.Hosting.Azure.Sql should be updated from `9.5.1` to `13.0.0` (*recommended for .NET 10.0*)
