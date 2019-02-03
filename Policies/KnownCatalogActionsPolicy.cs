// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownCatalogActionsPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Policies
{
    /// <inheritdoc />
    /// <summary>
    /// Defines the known catalog actions policy
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.Policy" />
    public class KnownCatalogActionsPolicy : Sitecore.Commerce.Plugin.Catalog.KnownCatalogActionsPolicy
    {
        public string MoveUpSellableItemImage { get; set; } = nameof(MoveUpSellableItemImage);

        public string MoveDownSellableItemImage { get; set; } = nameof(MoveDownSellableItemImage);
    }
}
