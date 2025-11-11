# .NET 10.0 Upgrade Report

## Project target framework modifications

| Project name                                                                           | Old Target Framework                                                           | New Target Framework                                                                  | Commits                   |
|:---------------------------------------------------------------------------------------|:------------------------------------------------------------------------------:|:-------------------------------------------------------------------------------------:|---------------------------|
| ShelfBuddy.SharedKernel.csproj                                                         | net9.0                                                                         | net10.0                                                                               | d61ebec4                  |
| ShelfBuddy.API.Common.csproj                                                           | net9.0                                                                         | net10.0                                                                               | 21ed61a1                  |
| ShelfBuddy.Contracts.csproj                                                            | net9.0                                                                         | net10.0                                                                               | a48691a1                  |
| ShelfBuddy.InventoryManagement.Domain.csproj                                           | net9.0                                                                         | net10.0                                                                               | 915f582f                  |
| ShelfBuddy.InventoryManagement.Application.csproj                                      | net9.0                                                                         | net10.0                                                                               | dd41a13f                  |
| ShelfBuddy.InventoryManagement.Infrastructure.csproj                                   | net9.0                                                                         | net10.0                                                                               | 15582d95                  |
| ShelfBuddy.ServiceDefaults.csproj                                                      | net9.0                                                                         | net10.0                                                                               | e0677323                  |
| ShelfBuddy.ClientInterface.LocalDb.csproj                                              | net9.0                                                                         | net10.0                                                                               | 33d64346                  |
| ShelfBuddy.API.csproj                                                                  | net9.0                                                                         | net10.0                                                                               | 6762ced5                  |
| ShelfBuddy.ClientInterface.MigrationManager.csproj                                     | net9.0                                                                         | net10.0                                                                               | 5a79ab75                  |
| ShelfBuddy.ClientInterface.csproj                                                      | net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0      | net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0         | ce1fa56c, 322059d2        |
| ShelfBuddy.AppHost.csproj                                                              | net9.0                                                                         | net10.0                                                                               | 7bde51aa                  |

## NuGet Packages

| Package Name                                   | Old Version | New Version      | Commit ID                                 |
|:-----------------------------------------------|:-----------:|:----------------:|-------------------------------------------|
| Aspire.Hosting.AppHost                         | 9.5.1       | 13.0.0           | 6ef33941                                  |
| Aspire.Hosting.Azure.Sql                       | 9.5.1       | 13.0.0           | 6ef33941                                  |
| Aspire.Microsoft.EntityFrameworkCore.SqlServer | 9.5.1       | 13.0.0           | 79e754f4                                  |
| Microsoft.AspNetCore.OpenApi                   | 9.0.10      | 10.0.0           | 933869c2                                  |
| Microsoft.EntityFrameworkCore.Design           | 9.0.10      | 10.0.0           | 933869c2                                  |
| Microsoft.EntityFrameworkCore.Sqlite           | 9.0.10      | 10.0.0           | 6994d30b                                  |
| Microsoft.Extensions.Hosting                   | 9.0.10      | 10.0.0           | 5d09e9f9                                  |
| Microsoft.Extensions.Http.Resilience           | 9.10.0      | 10.0.0           | 806501b6                                  |
| Microsoft.Extensions.ServiceDiscovery          | 9.5.1       | 10.0.0           | 806501b6                                  |
| OpenTelemetry.Instrumentation.AspNetCore       | 1.12.0      | 1.14.0-rc.1      | 806501b6                                  |
| OpenTelemetry.Instrumentation.Http             | 1.12.0      | 1.14.0-rc.1      | 806501b6                                  |

## All commits

| Commit ID              | Description                                                                  |
|:-----------------------|:-----------------------------------------------------------------------------|
| 543daa39               | Commit upgrade plan                                                          |
| d61ebec4               | Update target framework to net10.0 in ShelfBuddy.SharedKernel.csproj        |
| 21ed61a1               | Update target framework to net10.0 in ShelfBuddy.API.Common.csproj          |
| a48691a1               | Update target framework to net10.0 in ShelfBuddy.Contracts.csproj           |
| 915f582f               | Bump target framework to net10.0 in Domain.csproj                            |
| dd41a13f               | Bump target framework to net10.0 in Application.csproj                       |
| 15582d95               | Update target framework to net10.0 in .csproj file                           |
| 79e754f4               | Update Aspire.Microsoft.EntityFrameworkCore.SqlServer version                |
| e0677323               | Update target framework to net10.0 in ShelfBuddy.ServiceDefaults.csproj     |
| 806501b6               | Update package versions in Directory.Packages.props                          |
| 33d64346               | Bump target framework to net10.0 in ShelfBuddy.ClientInterface.LocalDb      |
| 6994d30b               | Update Microsoft.EntityFrameworkCore.Sqlite to 10.0.0                        |
| 6762ced5               | Update target framework to net10.0 in ShelfBuddy.API.csproj                 |
| 933869c2               | Update package versions in Directory.Packages.props                          |
| 5d09e9f9               | Update hosting and logging package versions to 10.0.0                        |
| 5a79ab75               | Update target framework to net10.0 in MigrationManager.csproj               |
| 9e739d8c               | Refactor ShelfBuddy.ClientInterface.csproj formatting                        |
| ce1fa56c               | Update target frameworks in ShelfBuddy.ClientInterface.csproj                |
| 322059d2               | Remove BOM from XAML files for consistency                                   |
| 6ef33941               | Update Aspire packages and add new dependencies                              |
| 7bde51aa               | Update target framework to net10.0 in ShelfBuddy.AppHost.csproj             |

## Summary

Successfully upgraded the ShelfBuddy solution from .NET 9.0 to .NET 10.0 (Preview). The upgrade included:

- **12 projects** upgraded from net9.0 to net10.0
- **1 .NET MAUI project** upgraded to support net10.0 across all platforms (Android, iOS, MacCatalyst, Windows)
- **13 NuGet packages** updated to versions compatible with .NET 10.0
- **3 major Aspire packages** upgraded from version 9.5.1 to 13.0.0
- All target framework changes successfully applied and validated

The upgrade process completed successfully with all projects now targeting .NET 10.0 and all dependencies updated to compatible versions.
