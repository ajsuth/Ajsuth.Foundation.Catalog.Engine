// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCategoryDetailsViewBlock.cs" company="Sitecore Corporation">
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
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get category details view pipeline block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
    ///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(Engine.CatalogConstants.Pipelines.Blocks.GetCategoryDetailsView)]
    public class GetCategoryDetailsViewBlock : GetCatalogNavigationViewBlock
    {
        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetCategoryDetailsViewBlock(CommerceCommander commander)
		    : base(commander)
		{
        }

        /// <summary>The execute.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The entity view can not be null");

            var policy = context.GetPolicy<KnownCatalogViewsPolicy>();
            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            var enablementPolicy = context.GetPolicy<Policies.CatalogFeatureEnablementPolicy>();
            if (!enablementPolicy.CatalogNavigationView
                    || string.IsNullOrEmpty(entityViewArgument?.ViewName)
                    || !entityViewArgument.ViewName.Equals(policy.Master, StringComparison.OrdinalIgnoreCase)
                    && !entityViewArgument.ViewName.Equals(policy.Details, StringComparison.OrdinalIgnoreCase)
                    && !entityViewArgument.ViewName.Equals(policy.ConnectCategory, StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(entityView).ConfigureAwait(false);
            }

            var category = entityViewArgument.Entity as Category;
            if (category == null)
            {
                return await Task.FromResult(entityView).ConfigureAwait(false);
            }

            await this.AddCatalogNavigationView(entityView, category, context).ConfigureAwait(false);

            return await Task.FromResult(entityView).ConfigureAwait(false);
        }
    }
}