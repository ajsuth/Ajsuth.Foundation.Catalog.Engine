// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCatalogHierarchyBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks
{
    using Ajsuth.Foundation.Catalog.Engine.Components;
    using Ajsuth.Foundation.Catalog.Engine.Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Ajsuth.Foundation.Catalog.Engine.CatalogConstants;

    /// <summary>
    /// Defines update catalog hierarchy pipeline block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
    ///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(Engine.CatalogConstants.Pipelines.Blocks.UpdateCatalogHierarchy)]
    public class UpdateCatalogHierarchyBlock : Sitecore.Commerce.Plugin.Catalog.UpdateCatalogHierarchyBlock
    {
		/// <summary>Gets or sets the commander.</summary>
		/// <value>The commander.</value>
		protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public UpdateCatalogHierarchyBlock(CommerceCommander commander, IFindEntityPipeline findEntityPipeline, IPersistEntityPipeline persistEntityPipeline)
		    : base(findEntityPipeline, persistEntityPipeline)
		{
            this.Commander = commander;
        }

        /// <summary>The execute.</summary>
        /// <param name="arg">The relationship argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="RelationshipArgument"/>.</returns>
        public override async Task<RelationshipArgument> Run(RelationshipArgument arg, CommercePipelineExecutionContext context)
        {
            if (context.CommerceContext.GetPolicy<CatalogNavigationPolicy>().UseUglySitecoreIds)
            {
                return await base.Run(arg, context).ConfigureAwait(false);
            }

            Condition.Requires(arg).IsNotNull($"{Name}: The argument can not be null");
            Condition.Requires(arg.TargetName).IsNotNullOrEmpty($"{Name}: The target name can not be null or empty");
            Condition.Requires(arg.SourceName).IsNotNullOrEmpty($"{Name}: The source name can not be null or empty");
            Condition.Requires(arg.RelationshipType).IsNotNullOrEmpty($"{Name}: The relationship type can not be null or empty");
            
            var relationshipTypes = new string[4] {
                RelationshipTypes.CatalogToCategory,
                RelationshipTypes.CategoryToCategory,
                RelationshipTypes.CatalogToSellableItem,
                RelationshipTypes.CategoryToSellableItem
            };
            if (!(relationshipTypes).Contains(arg.RelationshipType, StringComparer.OrdinalIgnoreCase))
            {
                return arg;
            }

            var sourceEntity = await Commander.Command<FindEntityCommand>().Process(context.CommerceContext, typeof(CatalogItemBase), arg.SourceName).ConfigureAwait(false) as CatalogItemBase;
            var stringList = new List<string>();
            if (arg.TargetName.Contains("|"))
            {
                string[] strArray = arg.TargetName.Split('|');
                stringList.AddRange(strArray);
            }
            else
            {
                stringList.Add(arg.TargetName);
            }

            var sourceChanged = new ValueWrapper<bool>(false);
            foreach (string entityId in stringList)
            {
                var catalogItemBase = await Commander.Command<FindEntityCommand>().Process(context.CommerceContext, typeof(CatalogItemBase), entityId).ConfigureAwait(false) as CatalogItemBase;
                if (sourceEntity != null && catalogItemBase != null)
                {
                    var changed = new ValueWrapper<bool>(false);
                    var categoryRelationshipsComponent = catalogItemBase.GetComponent<CategoryHierarchyComponent>();
                    switch (arg.RelationshipType)
                    {
                        case RelationshipTypes.CatalogToCategory:
                            sourceEntity.ChildrenCategoryList = UpdateHierarchy(arg, catalogItemBase.SitecoreId, sourceEntity.ChildrenCategoryList, sourceChanged);
                            catalogItemBase.ParentCatalogList = UpdateHierarchy(arg, sourceEntity.SitecoreId, catalogItemBase.ParentCatalogList, changed);
                            catalogItemBase.CatalogToEntityList = UpdateHierarchy(arg, sourceEntity.SitecoreId, catalogItemBase.CatalogToEntityList, changed);
                            categoryRelationshipsComponent.ParentEntityList = UpdateHierarchy(arg, sourceEntity.Id, categoryRelationshipsComponent.ParentEntityList, changed);
                            break;
                        case RelationshipTypes.CategoryToCategory:
                            sourceEntity.ChildrenCategoryList = UpdateHierarchy(arg, catalogItemBase.SitecoreId, sourceEntity.ChildrenCategoryList, sourceChanged);
                            catalogItemBase.ParentCategoryList = UpdateHierarchy(arg, sourceEntity.SitecoreId, catalogItemBase.ParentCategoryList, changed);
                            catalogItemBase.ParentCatalogList = UpdateHierarchy(arg, ExtractCatalogId(sourceEntity.Id), catalogItemBase.ParentCatalogList, changed);
                            categoryRelationshipsComponent.ParentEntityList = UpdateHierarchy(arg, sourceEntity.Id, categoryRelationshipsComponent.ParentEntityList, changed);
                            break;
                        case RelationshipTypes.CatalogToSellableItem:
                            sourceEntity.ChildrenSellableItemList = UpdateHierarchy(arg, catalogItemBase.SitecoreId, sourceEntity.ChildrenSellableItemList, sourceChanged);
                            catalogItemBase.ParentCatalogList = UpdateHierarchy(arg, sourceEntity.SitecoreId, catalogItemBase.ParentCatalogList, changed);
                            catalogItemBase.CatalogToEntityList = UpdateHierarchy(arg, sourceEntity.SitecoreId, catalogItemBase.CatalogToEntityList, changed);
                            categoryRelationshipsComponent.ParentEntityList = UpdateHierarchy(arg, sourceEntity.Id, categoryRelationshipsComponent.ParentEntityList, changed);
                            break;
                        case RelationshipTypes.CategoryToSellableItem:
                            sourceEntity.ChildrenSellableItemList = UpdateHierarchy(arg, catalogItemBase.SitecoreId, sourceEntity.ChildrenSellableItemList, sourceChanged);
                            catalogItemBase.ParentCategoryList = UpdateHierarchy(arg, sourceEntity.SitecoreId, catalogItemBase.ParentCategoryList, changed);
                            catalogItemBase.ParentCatalogList = UpdateHierarchy(arg, ExtractCatalogId(sourceEntity.Id), catalogItemBase.ParentCatalogList, changed);
                            categoryRelationshipsComponent.ParentEntityList = UpdateHierarchy(arg, sourceEntity.Id, categoryRelationshipsComponent.ParentEntityList, changed);
                            break;
                        default:
                            break;
                    }

                    if (changed.Value)
                    {
                        await Commander.PersistEntity(context.CommerceContext, catalogItemBase).ConfigureAwait(false);
                    }
                }
            }
            if (sourceChanged.Value)
            {
                await Commander.PersistEntity(context.CommerceContext, sourceEntity).ConfigureAwait(false);
            }

            return arg;
        }

        protected virtual string ExtractCatalogId(string id)
        {
            var strArray = id.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length < 3)
            {
                return string.Empty;
            }

            return GuidUtility.GetDeterministicGuidString(CommerceEntity.IdPrefix<Catalog>() + strArray[2]);
        }

        protected virtual string UpdateHierarchy(RelationshipArgument arg, string targetId, string rawChildren, ValueWrapper<bool> changed)
        {
            if (rawChildren == null)
            {
                rawChildren = string.Empty;
            }

            var list = rawChildren.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var mode = arg.Mode;
            if (mode.GetValueOrDefault() == RelationshipMode.Create & mode.HasValue && !list.Contains(targetId))
            {
                if (!changed.Value)
                {
                    changed.Value = true;
                }

                list.RemoveAll(c => c.Equals(targetId, StringComparison.OrdinalIgnoreCase));
                list.Add(targetId);
            }
            else if (mode.GetValueOrDefault() == RelationshipMode.Delete & mode.HasValue && list.Contains(targetId))
            {
                if (!changed.Value)
                {
                    changed.Value = true;
                }

                list.RemoveAll(c => c.Equals(targetId, StringComparison.OrdinalIgnoreCase));
            }

            return string.Join("|", list);
        }
    }
}