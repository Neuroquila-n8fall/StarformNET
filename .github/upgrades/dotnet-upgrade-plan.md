# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade StarformNet\StarformNET.csproj
4. Upgrade StarformNET.xUnitTests\StarformNET.xUnitTests.csproj
5. Upgrade StarformNET.GUI\StarformNET.GUI.csproj
6. Upgrade StarformNET.Console\StarformNET.Console.csproj


## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|


### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| System.Data.DataSetExtensions       |     4.5.0       |             | Functionality included in .NET 10; remove package reference and use framework-provided APIs |


### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### StarformNet\\StarformNET.csproj modifications

Project properties changes:
  - Target framework should be changed from `net7.0` to `net10.0`

NuGet packages changes:
  - `System.Data.DataSetExtensions` should be removed (functionality included in .NET 10)

Feature upgrades:
  - None identified automatically.

Other changes:
  - After changing target framework, run a full build and fix any new compiler errors due to API changes or obsoletions.

#### StarformNet.xUnitTests\\StarformNET.xUnitTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net7.0` to `net10.0`

NuGet packages changes:
  - No NuGet package changes detected for this project by analysis.

Feature upgrades:
  - None identified automatically.

Other changes:
  - After changing target framework, run unit tests and address any test failures.

#### StarformNet.GUI\\StarformNET.GUI.csproj modifications

Project properties changes:
  - Target framework should be changed from `net7.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `System.Data.DataSetExtensions` should be removed (functionality included in .NET 10)

Feature upgrades:
  - If the GUI project uses Windows-specific APIs, verify compatibility with the new SDK and target; update any Windows SDK or package references if needed.

Other changes:
  - After changing target framework, rebuild the GUI and test runtime behavior.

#### StarformNet.Console\\StarformNET.Console.csproj modifications

Project properties changes:
  - Target framework should be changed from `net7.0` to `net10.0`

NuGet packages changes:
  - `System.Data.DataSetExtensions` should be removed (functionality included in .NET 10)

Feature upgrades:
  - None identified automatically.

Other changes:
  - After changing target framework, run a full build and update any code that depends on removed or changed APIs.
