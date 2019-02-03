// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineBlock1Block.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The do action block that moves sellable item images up
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(Engine.CatalogConstants.Pipelines.Blocks.DoActionMoveUpSellableItemImage)]
    public class DoActionMoveUpSellableItemImageBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>
		/// Gets or sets the commander.
		/// </summary>
		/// <value>
		/// The commander.
		/// </value>
		protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks.DoActionMoveUpSellableItemImageBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public DoActionMoveUpSellableItemImageBlock(CommerceCommander commander)
          : base(null)
        {
            this.Commander = commander;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="entityView">
        /// The <see cref="EntityView"/>.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (string.IsNullOrEmpty(entityView?.Action) ||
                    string.IsNullOrEmpty(entityView.EntityId) ||
                    (string.IsNullOrEmpty(entityView.ItemId) ||
                    !entityView.Action.Equals(context.GetPolicy<Policies.KnownCatalogActionsPolicy>().MoveUpSellableItemImage, StringComparison.OrdinalIgnoreCase)))
            {
                return entityView;
            }

            var sellableItem = context.CommerceContext.GetObjects<SellableItem>().FirstOrDefault(s => s.Id.Equals(entityView.EntityId, StringComparison.OrdinalIgnoreCase));
            if (sellableItem == null)
            {
                return entityView;
            }
            
            var imageDetails = entityView.ItemId.Split('|');
            var variationId = imageDetails?[0] ?? string.Empty;
            var imageId = imageDetails?[1] ?? string.Empty;
            var imageComponent = sellableItem.GetComponent<ImagesComponent>(variationId, false);
            if (!imageComponent.Images.Any(i => i.Equals(imageId, StringComparison.OrdinalIgnoreCase)))
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[1] { "ItemId" },
                    "Invalid or missing value for property 'ItemId'.");

                return entityView;
            }

            var index = imageComponent.Images.IndexOf(imageId);
            if (index <= 0)
            {
                return entityView;
            }

            imageComponent.Images.Remove(imageId);
            imageComponent.Images.Insert(index - 1, imageId);
            await this.Commander.PersistEntity(context.CommerceContext, sellableItem);

            return entityView;
        }
    }
}