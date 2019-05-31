// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCatalogNavigationViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks
{
    using Ajsuth.Foundation.Catalog.Engine.Components;
    using Ajsuth.Foundation.Catalog.Engine.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the get catalog navigation view pipeline block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
    ///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(Engine.CatalogConstants.Pipelines.Blocks.GetSellableItemDetailsView)]
    public class GetCatalogNavigationViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
		/// <summary>Gets or sets the commander.</summary>
		/// <value>The commander.</value>
		protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public GetCatalogNavigationViewBlock(CommerceCommander commander)
		    : base(null)
		{
            this.Commander = commander;
        }

        /// <summary>The execute.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            return await Task.FromResult(entityView).ConfigureAwait(false);
        }

        protected virtual async Task AddCatalogNavigationView(EntityView entityView, CatalogItemBase catalogItem, CommercePipelineExecutionContext context)
        {
            var parentCategoriesView = new EntityView { Name = "Catalog Navigation", Icon = "elements_hierarchy", UiHint = "Table" };
            entityView.ChildViews.Insert(0, parentCategoriesView);

            var breadcrumbs = await this.GetBreadcrumbs(catalogItem, context).ConfigureAwait(false);
            breadcrumbs.Sort();
            foreach (var breadcrumb in breadcrumbs)
            {
                var breadcrumbView = new EntityView { Name = "Breadcrumbs", Icon = null };
                breadcrumbView.Properties.Add(new ViewProperty() { Name = "Breadcrumb", Value = breadcrumb, UiType = "Html" });
                parentCategoriesView.ChildViews.Add(breadcrumbView);
            }
        }

        protected virtual async Task<List<string>> GetBreadcrumbs(CatalogItemBase catalogItem, CommercePipelineExecutionContext context)
        {
            var breadcrumbs = new List<string>();
            var entityIdList = this.GetParentEntityList(catalogItem, context);
            if (entityIdList == null || entityIdList.Count == 0)
            {
                breadcrumbs.Add(this.GetEntityLink(catalogItem));

                return breadcrumbs;
            }

            foreach (var id in entityIdList)
            {
                var parentCatalogItem = await this.GetParentCatalogItem(id, context).ConfigureAwait(false);
                if (parentCatalogItem == null)
                {
                    continue;
                }
                var parentBreadcrumbs = await GetBreadcrumbs(parentCatalogItem, context).ConfigureAwait(false);
                foreach (var breadcrumb in parentBreadcrumbs)
                {
                    breadcrumbs.Add($"{breadcrumb} > {this.GetEntityLink(catalogItem)}");
                }
            }

            return await Task.FromResult(breadcrumbs).ConfigureAwait(false);
        }
        
        protected virtual string GetEntityLink(CatalogItemBase catalogItem)
        {
            return $"<a href=\"/entityView/Master/{catalogItem.EntityVersion}/{catalogItem.Id}\">{catalogItem.DisplayName}</a>";
        }

        protected virtual List<string> GetParentEntityList(CatalogItemBase catalogItem, CommercePipelineExecutionContext context)
        {
            if (context.CommerceContext.GetPolicy<CatalogNavigationPolicy>().UseUglySitecoreIds)
            {
                var parentList = this.ToParentList(catalogItem.ParentCategoryList);
                if ((parentList == null || !parentList.Any()) && !(catalogItem is Catalog))
                {
                    return new List<string>() { this.GetCatalogId(catalogItem) };
                }
                return parentList;
            }
            else
            {
                var categoryRelationshipsComponent = catalogItem.GetComponent<CategoryHierarchyComponent>();
                return this.ToParentList(categoryRelationshipsComponent.ParentEntityList);
            }
        }

        protected virtual List<string> ToParentList(string parentEntityList)
        {
            return parentEntityList?.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        
        public virtual string GetCatalogId(CatalogItemBase catalogItem)
        {
            if (string.IsNullOrWhiteSpace(catalogItem.FriendlyId))
            {
                return null;
            }

            var strArray = catalogItem.FriendlyId.Split('-');
            return strArray[0]?.ToEntityId<Catalog>();
        }

        protected virtual async Task<CatalogItemBase> GetParentCatalogItem(string id, CommercePipelineExecutionContext context)
        {
            if (context.CommerceContext.GetPolicy<CatalogNavigationPolicy>().UseUglySitecoreIds)
            {
                if (!id.IsEntityId<Catalog>())
                {
                    var dataSet = await Commander.Command<GetMappingsForIdFromDbCommand>().Process(context.CommerceContext, context.CommerceContext.Environment.Name, id).ConfigureAwait(false);
                    id = dataSet.Tables[0]?.Rows[0]?["EntityId"]?.ToString();
                    if (id == null)
                    {
                        return null;
                    }
                }

                return await Commander.GetEntity<CatalogItemBase>(context.CommerceContext, id).ConfigureAwait(false);
            }
            else
            {
                return await Commander.GetEntity<CatalogItemBase>(context.CommerceContext, id).ConfigureAwait(false);
            }
        }
    }
}