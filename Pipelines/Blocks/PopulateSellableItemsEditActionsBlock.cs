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
    using System.Threading.Tasks;

    /// <summary>
    /// The pipeline block that populates the sellable items edit actions
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(Engine.CatalogConstants.Pipelines.Blocks.PopulateSellableItemsEditActions)]
    public class PopulateSellableItemsEditActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks.PopulateSellableItemsEditActionsBlock" /> class.</summary>
        public PopulateSellableItemsEditActionsBlock()
          : base(null)
        {
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
            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
			var enablementPolicy = context.GetPolicy<Policies.CatalogFeatureEnablementPolicy>();
			if (!enablementPolicy.MoveImageActions
					|| !(entityViewArgument?.Entity is SellableItem)
					|| !string.IsNullOrEmpty(entityViewArgument.ForAction))
            {
                return entityView;
            }

            var entity = (SellableItem)entityViewArgument.Entity;
            var viewsPolicy = context.GetPolicy<Policies.KnownCatalogViewsPolicy>();
            var actionsPolicy = context.GetPolicy<Policies.KnownCatalogActionsPolicy>();
            var entityViewActionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            if (entityView.Name.Equals(viewsPolicy.Images, StringComparison.OrdinalIgnoreCase))
            {
                entityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = actionsPolicy.MoveUpSellableItemImage,
                    DisplayName = "Move Image Up",
                    Description = "Moves an image up",
                    IsEnabled = entity.GetComponent<ImagesComponent>().Images.Count > 1,
                    RequiresConfirmation = true,
                    Icon = "arrow_up"
                });

                entityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = actionsPolicy.MoveDownSellableItemImage,
                    DisplayName = "Move Image Down",
                    Description = "Moves an image down",
                    IsEnabled = entity.GetComponent<ImagesComponent>().Images.Count > 1,
                    RequiresConfirmation = true,
                    Icon = "arrow_down"
                });
            }

            return await Task.FromResult(entityView).ConfigureAwait(false);
        }
    }
}
