// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogConstants.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine
{
    /// <summary>
    /// The CatalogConstants.
    /// </summary>
    public class CatalogConstants
    {
        /// <summary>
        /// The names of the pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The names of the pipeline blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The do action move down sellable item image block name.
                /// </summary>
                public const string DoActionMoveDownSellableItemImage = "Catalog.Block.DoActionMoveDownSellableItemImage";

                /// <summary>
                /// The do action move up sellable item image block name.
                /// </summary>
                public const string DoActionMoveUpSellableItemImage = "Catalog.Block.DoActionMoveUpSellableItemImage";

                /// <summary>
                /// The get catalog navigation view block name.
                /// </summary>
                public const string GetCatalogNavigationView = "Catalog.Block.GetCatalogNavigationView";

                /// <summary>
                /// The get category details view block name.
                /// </summary>
                public const string GetCategoryDetailsView = "Catalog.Block.GetCategoryDetailsView";

                /// <summary>
                /// The get sellable item details view block name.
                /// </summary>
                public const string GetSellableItemDetailsView = "Catalog.Block.GetSellableItemDetailsView";

                /// <summary>
                /// The get sellable item status block name.
                /// </summary>
                public const string GetSellableItemStatusViewBlock = "Catalog.Block.GetSellableItemStatusViewBlock";

                /// <summary>
                /// The populate sellable items edit actions block name.
                /// </summary>
                public const string PopulateSellableItemsEditActions = "Catalog.Block.PopulateSellableItemsEditActions";

                /// <summary>
                /// The update catalog hierarchy block name.
                /// </summary>
                public const string UpdateCatalogHierarchy = "Catalog.Block.UpdateCatalogHierarchy";
            }
        }

        public static class InventoryStatus
        {
            public const string InStock = "In Stock";
            public const string OnPreOrder = "On Pre-Order";
            public const string OnBackOrder = "On Back Order";
            public const string OutOfStock = "Out of Stock";
            public const string Perpetual = "Perpetual";
            public const string NotAvailable = "Not Available";
        }

        /// <summary>
        /// The names of relationship types
        /// </summary>
        public static class RelationshipTypes
        {
            /// <summary>
            /// The catalog to category relationship
            /// </summary>
            public const string CatalogToCategory = "CatalogToCategory";

            /// <summary>
            /// The category to category relationship
            /// </summary>
            public const string CategoryToCategory = "CategoryToCategory";

            /// <summary>
            /// The catalog to sellable item relationship
            /// </summary>
            public const string CatalogToSellableItem = "CatalogToSellableItem";

            /// <summary>
            /// The category to sellable item relationship
            /// </summary>
            public const string CategoryToSellableItem = "CategoryToSellableItem";
        }

    }
}