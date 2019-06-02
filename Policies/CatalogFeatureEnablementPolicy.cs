// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogFeatureEnablementPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Policies
{
	using Sitecore.Commerce.Core;
    /// <inheritdoc />
    /// <summary>
    /// Defines the catalog feature enablement policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class CatalogFeatureEnablementPolicy : Policy
    {
        public bool CatalogNavigationView { get; set; }

        public bool MoveImageActions { get; set; }

		public bool StatusViews { get; set; }

		public bool VariationProperties { get; set; }
	}
}
