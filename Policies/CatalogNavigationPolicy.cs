// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogNavigationPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Policies
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// Defines the catalog navigation policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class CatalogNavigationPolicy : Policy
    {
        public bool UseUglySitecoreIds { get; set; } = true;
    }
}
