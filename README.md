# Extended Sitecore Commerce Catalog
Custom Sitecore Commerce catalog plugin project with extended functionality.

## Features
| Feature                 | Description | Policy Property |
| ----------------------- | ----------- | --------------- |
| Order Image Actions     | Abiltity to order sellable item and variant images. | MoveImageActions |
| Status Entity Views     | Site-Ready Status, Pricing Status, and Inventory Status entity views added to the sellable item and variant views. | StatusViews |
| Catalog Navigation View | Catalog Navigation Entity View added to category and sellable item views to see descendent associations| CatalogNavigationView |

## Known Issues:
| Feature                 | Description | Issue |
| ----------------------- | ----------- | ----- |
| Status Entity Views     | Bundles not currently supported in Status views. | [#2](https://github.com/ajsuth/Ajsuth.Foundation.Catalog.Engine/issues/2) |
| Status Entity Views     | Status views currently only render status for latest entity version.| [#3](https://github.com/ajsuth/Ajsuth.Foundation.Catalog.Engine/issues/3) |

## Installation Instructions
1. Download the repository.
2. Add the **Ajsuth.Foundation.Catalog.Engine.csproj** to the _**Sitecore Commerce Engine**_ solution.
3. In the _**Sitecore Commerce Engine**_ project, add a reference to the **Ajsuth.Foundation.Catalog.Engine** project.
4. Run the _**Sitecore Commerce Engine**_ from Visual Studio or deploy the solution and run from IIS.

## Disabling Features
In the environment configuration files, add the **CatalogFeatureEnablementPolicy** and set the desired feature to false. (See the Policy Property column in Features). For example:
```javascript
{
	"$type": "Ajsuth.Foundation.Catalog.Engine.Policies.CatalogFeatureEnablementPolicy, Ajsuth.Foundation.Catalog.Engine",
	"MoveImageActions": false
}
```

## Disclaimer
The code provided in this repository is sample code only. It is not intended for production usage and not endorsed by Sitecore.
Both Sitecore and the code author do not take responsibility for any issues caused as a result of using this code.
No guarantee or warranty is provided and code must be used at own risk.
