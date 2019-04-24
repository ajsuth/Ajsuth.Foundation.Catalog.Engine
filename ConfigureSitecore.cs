// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

                .ConfigurePipeline<IDoActionPipeline>(pipeline => pipeline
                    .Add<Pipelines.Blocks.DoActionMoveUpSellableItemImageBlock>().After<DoActionRemoveSellableItemImageBlock>()
                    .Add<Pipelines.Blocks.DoActionMoveDownSellableItemImageBlock>().After<DoActionRemoveSellableItemImageBlock>()
                )

                .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(pipeline => pipeline
                    .Add<Pipelines.Blocks.PopulateSellableItemsEditActionsBlock>().After<PopulateSellableItemsEditActionsBlock>()
                )

                .ConfigurePipeline<IGetEntityViewPipeline>(pipeline => pipeline
                    .Add<Pipelines.Blocks.GetSellableItemStatusViewBlock>().After<GetSellableItemDetailsViewBlock>()
                )

            );

            services.RegisterAllCommands(assembly);
        }
    }
}