# Extended Sitecore Commerce Catalog
Custom Sitecore Commerce catalog plugin project with extended functionality for the Business Tools.

## Features
- [Order Image Actions](#order-image-actions)
- [Status Entity Views](#status-entity-views)
- [Catalog Navigation View](#catalog-navigation-view)
- [Sellable Item Variation Properties](#sellable-item-variation-properties)
- [Variant Sellable Item Entity Link](#variant-sellable-item-entity-link)
### Order Image Actions
Adds actions, **Move Image Up** and **Move Image Down**, to the **Images** entity view, to provide the business user the ability to order sellable item and variant images.

**Enablement Policy Property:** MoveImageActions

![Order Image Actions](/images/order-image-actions.png)


### Status Entity Views
Adds **Site-Ready Status**, **Pricing Status**, and **Inventory Status** entity views added to the sellable item and variant page views. These views assist the business user in assessing whether the sellable item/variant has been correctly configured to be able to be purchased in the storefront.

In the **Site-Ready Status** view, the **Price Status** field is based on the **Default Currency** pricing.

**Note:** Customising around storefront/catalog associations, pricing, inventory, etc. may render these values inaccurate.

**Enablement Policy Property:** StatusViews

![Site-Ready Status entity view](/images/site-ready-status-view.png)

![Pricing Status entity view](/images/pricing-status-view.png)

![Inventory Status entity view](/images/inventory-status-view.png)

### Catalog Navigation View
Adds the **Catalog Navigation** entity view to category and sellable item page views to assist the business user with Merchandising navigation and identifying the descendent associations. 

**Enablement Policy Property:** CatalogNavigationView

![Catalog Breadcrumbs entity view for Category entities](/images/catalog-breadcrumbs-category.png)

_Category entity view._

![Catalog Breadcrumbs entity view for Sellable Item entities](/images/catalog-breadcrumbs-sellable-item.png)

_Sellable Item entity view._

### Sellable Item Variation Properties
Adds the properties from the **VariationPropertiesPolicy** to the Sellable Item's Variants entity view. 

**Enablement Policy Property:** VariationProperties

![Variation Properites in the Sellable Item's Variants entity view](/images/variation-properties.png)

_Variants entity view._

### Variant Sellable Item Entity Link
Adds a **Sellable Item** entity view to the variant page view to provide a direct link back to the variant's owning sellable item.

**Enablement Policy Property:** RenderVariantSellableItemLink

![Sellable Item entity view in Variant page view](/images/variant-sellable-item-entity-view.png)

_Sellable Item entity view._

## Enabling Features
In the environment configuration files, add the **CatalogFeatureEnablementPolicy** and set the desired features to `true`. (See the **Enablement Policy Property** value under each feature). For example:
```javascript
{
	"$type": "Ajsuth.Foundation.Catalog.Engine.Policies.CatalogFeatureEnablementPolicy, Ajsuth.Foundation.Catalog.Engine",
	"MoveImageActions": true
}
```

## Installation Instructions
1. Download the repository.
2. Add the **Ajsuth.Foundation.Catalog.Engine.csproj** to the _**Sitecore Commerce Engine**_ solution.
3. In the _**Sitecore Commerce Engine**_ project, add a reference to the **Ajsuth.Foundation.Catalog.Engine** project.
4. Enable desired features, following [Enabling Features](#enabling-features).
5. Run the _**Sitecore Commerce Engine**_ from Visual Studio or deploy the solution and run from IIS.
6. Run the **Bootstrap** command on the _**Sitecore Commerce Engine**_.  

## Known Issues
| Feature                 | Description | Issue |
| ----------------------- | ----------- | ----- |
| Status Entity Views     | Bundles not currently supported in Status views. | [#2](https://github.com/ajsuth/Ajsuth.Foundation.Catalog.Engine/issues/2) |
| Status Entity Views     | Status views currently only render status for latest entity version, regardless of which entity version has is active.| [#3](https://github.com/ajsuth/Ajsuth.Foundation.Catalog.Engine/issues/3) |
| Catalog Navigation View | Associating a category to the catalog and another category will prevent the catalog-category breadcrumb from displaying in the list of breadcrumbs. | [#5](https://github.com/ajsuth/Ajsuth.Foundation.Catalog.Engine/issues/5) |

## Disclaimer
The code provided in this repository is sample code only. It is not intended for production usage and not endorsed by Sitecore.
Both Sitecore and the code author do not take responsibility for any issues caused as a result of using this code.
No guarantee or warranty is provided and code must be used at own risk.
