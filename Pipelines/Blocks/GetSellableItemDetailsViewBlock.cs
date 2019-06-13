// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSellableItemDetailsViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
	using System.Linq;
	using System.Threading.Tasks;

    /// <summary>
    /// Defines the get sellable item details view pipeline block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
    ///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(Engine.CatalogConstants.Pipelines.Blocks.GetSellableItemDetailsView)]
    public class GetSellableItemDetailsViewBlock : GetCatalogNavigationViewBlock
    {
        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetSellableItemDetailsViewBlock(CommerceCommander commander)
		    : base(commander)
		{
        }

        /// <summary>The execute.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            var policy = context.CommerceContext.GetPolicy<KnownCatalogViewsPolicy>();
            var enablementPolicy = context.GetPolicy<Policies.CatalogFeatureEnablementPolicy>();
            if (string.IsNullOrEmpty(entityViewArgument?.ViewName)
                    || !entityViewArgument.ViewName.Equals(policy.Master, StringComparison.OrdinalIgnoreCase)
                    && !entityViewArgument.ViewName.Equals(policy.Variant, StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(entityView).ConfigureAwait(false);
            }

            var sellableItem = entityViewArgument.Entity as SellableItem;
            if (sellableItem == null)
            {
                return await Task.FromResult(entityView).ConfigureAwait(false);
            }

			if (enablementPolicy.CatalogNavigationView)
			{
				await this.AddCatalogNavigationView(entityView, sellableItem, context).ConfigureAwait(false);
			}

			if (enablementPolicy.RenderVariantSellableItemLink
					&& entityViewArgument.ViewName.Equals(policy.Variant, StringComparison.OrdinalIgnoreCase)
					&& string.IsNullOrEmpty(entityViewArgument.ForAction))
			{
				this.UpdateVariantView(entityView, sellableItem, context);
			}

			var isBundle = sellableItem.HasComponent<BundleComponent>();
			if (enablementPolicy.VariationProperties
					&& !isBundle
					&& entityViewArgument.ViewName.Equals(policy.Master, StringComparison.OrdinalIgnoreCase)
					&& string.IsNullOrEmpty(entityViewArgument.ForAction))
			{
				var variantsView = entityView.ChildViews.FirstOrDefault(c => c.Name == policy.SellableItemVariants) as EntityView;
				UpdateVariantsView(variantsView, sellableItem, context);
			}

			return await Task.FromResult(entityView).ConfigureAwait(false);
        }

		/// <summary>
		/// Updates variant entity view
		/// </summary>
		/// <param name="variantView">The variant view.</param>
		/// <param name="sellableItem">The sellable item.</param>
		/// <param name="context">The context.</param>
		protected virtual void UpdateVariantView(EntityView variantView, SellableItem sellableItem, CommercePipelineExecutionContext context)
		{
			if (variantView == null || sellableItem == null)
			{
				return;
			}

			var sellableItemView = new EntityView() { Name = "SellableItem", UiHint = "Table" };
			variantView.ChildViews.Insert(0, sellableItemView);

			var sellableItemMasterView = new EntityView {
				Name = "MasterSellableItem",
				Icon = null,
				EntityId = sellableItem.Id,
				EntityVersion = sellableItem.EntityVersion,
				ItemId = sellableItem.Id,
			};
			sellableItemMasterView.Properties.Add(
				new ViewProperty()
				{
					Name = "ProductId",
					RawValue = sellableItem.ProductId,
					IsReadOnly = true,
					UiType = "EntityLink"
				});
			sellableItemMasterView.Properties.Add(
				new ViewProperty()
				{
					Name = "Name",
					RawValue = sellableItem.Name,
					IsReadOnly = true
				});
			sellableItemMasterView.Properties.Add(
				new ViewProperty()
				{
					Name = "DisplayName",
					RawValue = sellableItem.DisplayName,
					IsReadOnly = true
				});
			sellableItemView.ChildViews.Add(sellableItemMasterView);
		}

		/// <summary>
		/// Updates variants entity view
		/// </summary>
		/// <param name="variantsView">The variants view.</param>
		/// <param name="sellableItem">The sellable item.</param>
		/// <param name="context">The context.</param>
		protected virtual void UpdateVariantsView(EntityView variantsView, SellableItem sellableItem, CommercePipelineExecutionContext context)
		{
			if (variantsView == null)
			{
				return;
			}

			var variations = sellableItem.GetComponent<ItemVariationsComponent>().ChildComponents.OfType<ItemVariationComponent>().ToList();
			if (variations != null && variations.Count <= 0)
			{
				return;
			}

			var variationPropertyPolicy = context.CommerceContext.Environment.GetPolicy<VariationPropertyPolicy>();
			foreach (var variation in variations)
			{
				var variationView = variantsView.ChildViews.FirstOrDefault(c => ((EntityView)c).ItemId == variation.Id) as EntityView;
				PopulateVariationProperties(variationView, variation, variationPropertyPolicy);
			}
		}

		/// <summary>
		/// Populates the variation properties in the entity view
		/// </summary>
		/// <param name="variationView">The variation view.</param>
		/// <param name="variation">The item variation component.</param>
		/// <param name="variationPropertyPolicy">The variation property policy.</param>
		protected virtual void PopulateVariationProperties(EntityView variationView, ItemVariationComponent variation, VariationPropertyPolicy variationPropertyPolicy)
		{
			if (variationView == null)
			{
				return;
			}

			foreach (var variationProperty in variationPropertyPolicy.PropertyNames)
			{
				var property = GetVariationProperty(variation, variationProperty);

				var insertIndex = variationView.Properties.Count > 0 ? variationView.Properties.Count - 1 : 0;
				variationView.Properties.Insert(insertIndex, new ViewProperty
				{
					Name = variationProperty,
					RawValue = property ?? string.Empty,
					IsReadOnly = true
				});
			}
		}

		/// <summary>
		/// Gets the variation property
		/// </summary>
		/// <param name="variationComponent">The item variation component.</param>
		/// <param name="variationProperty">The name of the variation property.</param>
		/// <returns></returns>
		protected virtual object GetVariationProperty(ItemVariationComponent variationComponent, string variationProperty)
		{
			foreach (var component in variationComponent.ChildComponents)
			{
				var property = component.GetType().GetProperty(variationProperty);
				if (property != null)
				{
					return property.GetValue(component);
				}
			}

			return null;
		}
	}
}