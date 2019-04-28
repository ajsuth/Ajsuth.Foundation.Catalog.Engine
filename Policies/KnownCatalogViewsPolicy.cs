// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownCatalogActionsPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Policies
{
    /// <inheritdoc />
    /// <summary>
    /// Defines the known catalog views policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class KnownCatalogViewsPolicy : Sitecore.Commerce.Plugin.Catalog.KnownCatalogViewsPolicy
	{
		public string Images { get; set; } = nameof(Images);
		public string SiteReadyStatus { get; set; } = nameof(SiteReadyStatus);
		public string PricingStatus { get; set; } = nameof(PricingStatus);
		public string InventoryStatus { get; set; } = nameof(InventoryStatus);
	}
}
