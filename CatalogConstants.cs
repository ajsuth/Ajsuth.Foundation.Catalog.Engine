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
                /// The populate sellable items edit actions block name.
                /// </summary>
                public const string PopulateSellableItemsEditActions = "Catalog.Block.PopulateSellableItemsEditActions";
            }
        }
    }
}