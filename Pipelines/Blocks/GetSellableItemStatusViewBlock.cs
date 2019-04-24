// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSellableItemStatusViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Foundation.Catalog.Engine.Pipelines.Blocks
{
	using Ajsuth.Foundation.Catalog.Engine.Extensions;
	using Ajsuth.Foundation.Catalog.Engine.Models;
	using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
	using Sitecore.Commerce.Plugin.Availability;
	using Sitecore.Commerce.Plugin.Catalog;
	using Sitecore.Commerce.Plugin.Inventory;
	using Sitecore.Commerce.Plugin.Pricing;
	using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines the get sellable item status view pipeline block
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
	///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
	///     </cref>
	/// </seealso>
	[PipelineDisplayName("CatalogConstants.Pipelines.Blocks.GetSellableItemStatusViewBlock")]
	public class GetSellableItemStatusViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
	{
		/// <summary>Gets or sets the commander.</summary>
		/// <value>The commander.</value>
		protected CommerceCommander Commander { get; set; }

		/// <inheritdoc />
		/// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
		/// <param name="commander">The commerce commander.</param>
		public GetSellableItemStatusViewBlock(CommerceCommander commander)
			: base(null)
		{
			this.Commander = commander;
		}

		/// <summary>The run.</summary>
		/// <param name="entityView">The <see cref="EntityView"/>.</param>
		/// <param name="context">The context.</param>
		/// <returns>The <see cref="EntityView"/>.</returns>
		public async override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
		{
			Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument can not be null");

			var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
			var policy = context.CommerceContext.GetPolicy<KnownCatalogViewsPolicy>();
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
			
			var inventoryStatusView = new EntityView { Name = "Inventory Status", Icon = "barrels", UiHint = "Table" };
			entityView.ChildViews.Insert(0, inventoryStatusView);

			var pricingStatusView = new EntityView { Name = "Pricing Status", Icon = "moneybag_dollar", UiHint = "Table" };
			entityView.ChildViews.Insert(0, pricingStatusView);

			var siteReadyStatusView = new EntityView { Name = "Site-Ready Status", Icon = "clipboard_checks", UiHint = "Table" };
			entityView.ChildViews.Insert(0, siteReadyStatusView);
			
			var component = sellableItem.HasComponent<CatalogsComponent>() ? sellableItem.GetComponent<CatalogsComponent>() : null;
			if (component == null)
			{
				return await Task.FromResult(entityView).ConfigureAwait(false);
			}

			var variant = sellableItem.GetVariation(entityView.ItemId);
			var catalogComponents = component.GetComponents<CatalogComponent>();
			foreach (var catalogComponent in catalogComponents)
			{
				var sellableItemsPerCurrency = await GetSellableItemPerCurrency(catalogComponent.Name, sellableItem.FriendlyId, variant?.Id, context).ConfigureAwait(false);
				var sellableItemForCurrentCurrency = sellableItemsPerCurrency.FirstOrDefault(item => item.ListPrice.CurrencyCode.Equals(context.CommerceContext.CurrentCurrency(), StringComparison.InvariantCultureIgnoreCase));

				await AddSiteReadyStatusProperties(siteReadyStatusView, catalogComponent.Name, sellableItemForCurrentCurrency, variant?.Id, context).ConfigureAwait(false);
				await AddPricingStatusProperties(pricingStatusView, catalogComponent.Name, sellableItemsPerCurrency, variant?.Id, context).ConfigureAwait(false);
				await AddInventoryStatusProperties(inventoryStatusView, catalogComponent.Name, sellableItemForCurrentCurrency, variant?.Id, context).ConfigureAwait(false);
			}

			return await Task.FromResult(entityView).ConfigureAwait(false);
		}
		

		protected async virtual Task AddSiteReadyStatusProperties(EntityView siteReadyStatusView, string catalogName, SellableItem sellableItem, string variantId, CommercePipelineExecutionContext context)
		{
			var catalog = await Commander.Command<GetCatalogCommand>().Process(context.CommerceContext, catalogName).ConfigureAwait(false);
			var priceBookName = catalog?.PriceBookName;
			var inventorySetName = catalog?.DefaultInventorySetName;

			var publishStatus = GetPublishedStatus(sellableItem);
			var enabledStatus = GetEnabledStatus(sellableItem, variantId);
			var pricingStatus = GetPricingStatus(sellableItem, variantId);
			var inventoryStatus = await GetInventoryStatus(inventorySetName, sellableItem, variantId, context).ConfigureAwait(false);
			var inventoryAvailability = ToInventoryAvailability(inventoryStatus);
			var complete = publishStatus
							&& enabledStatus
							&& pricingStatus
							&& inventoryAvailability;
			var icon = sellableItem.IsBundle ? "question" : complete ? "check" : "delete";
			var view = new EntityView { Name = "Catalog Item Status", Icon = icon };

			view.Properties.Add(new ViewProperty() { Name = "Catalog", Value = catalogName });
			view.Properties.Add(new ViewProperty() { Name = "Published Status", RawValue = publishStatus });
			view.Properties.Add(new ViewProperty() { Name = "Enabled Status", RawValue = sellableItem.IsBundle ? "Not Implemented" : enabledStatus.ToString().ToLower() });
			view.Properties.Add(new ViewProperty() { Name = "Price Status", RawValue = pricingStatus });
			view.Properties.Add(new ViewProperty() { Name = "Default Currency", RawValue = context.CommerceContext.CurrentCurrency() });
			view.Properties.Add(new ViewProperty() { Name = "Price Book Name", RawValue = priceBookName });
			view.Properties.Add(new ViewProperty() { Name = "Inventory Availability", RawValue = sellableItem.IsBundle ? "Not Implemented" : inventoryAvailability.ToString().ToLower() });
			view.Properties.Add(new ViewProperty() { Name = "Inventory Set Name", RawValue = inventorySetName });

			siteReadyStatusView.ChildViews.Add(view);
		}

		protected async virtual Task AddPricingStatusProperties(EntityView pricingStatusView, string catalogName, List<SellableItem> sellableItems, string variantId, CommercePipelineExecutionContext context)
		{
			foreach (var sellableItem in sellableItems)
			{
				var pricingModel = await CreatePricingModel(sellableItem, variantId, context).ConfigureAwait(false);
				if (pricingModel == null)
				{
					continue;
				}
				
				var catalogCurrencyStatusView = new EntityView { Name = "Item Status", Icon = pricingModel.SellPrice != null ? "check" : "delete" };

				catalogCurrencyStatusView.Properties.Add(new ViewProperty() { Name = "Catalog", Value = catalogName });
				catalogCurrencyStatusView.Properties.Add(new ViewProperty() { Name = "Currency", Value = pricingModel.ListPrice?.CurrencyCode });
				catalogCurrencyStatusView.Properties.Add(new ViewProperty() { Name = "List Price", RawValue = pricingModel.ListPrice?.Amount });
				catalogCurrencyStatusView.Properties.Add(new ViewProperty() { Name = "Sell Price", RawValue = pricingModel.SellPrice?.Amount });
				catalogCurrencyStatusView.Properties.Add(new ViewProperty() { Name = "List Price Messages", RawValue = pricingModel.ListPriceMessages, UiType = "List" });
				catalogCurrencyStatusView.Properties.Add(new ViewProperty() { Name = "Sell Price Messages", RawValue = pricingModel.SellPriceMessages, UiType = "List" });

				pricingStatusView.ChildViews.Add(catalogCurrencyStatusView);
			}
		}

		protected async virtual Task AddInventoryStatusProperties(EntityView inventoryStatusView, string catalogName, SellableItem sellableItem, string variantId, CommercePipelineExecutionContext context)
		{
			var catalog = await Commander.Command<GetCatalogCommand>().Process(context.CommerceContext, catalogName).ConfigureAwait(false);
			var inventorySetName = catalog?.DefaultInventorySetName;
			if (catalog == null || string.IsNullOrWhiteSpace(catalog.DefaultInventorySetName))
			{
				return;
			}

			var inventoryStatus = await GetInventoryStatus(inventorySetName, sellableItem, variantId, context).ConfigureAwait(false);
			var icon = sellableItem.IsBundle ? "question" : inventoryStatus != Engine.CatalogConstants.InventoryStatus.NotAvailable && inventoryStatus != Engine.CatalogConstants.InventoryStatus.OutOfStock ? "check" : "delete";
			var view = new EntityView { Name = "Catalog Item Status", Icon = icon };

			view.Properties.Add(new ViewProperty() { Name = "Inventory Set Name", Value = inventorySetName });
			view.Properties.Add(new ViewProperty() { Name = "Inventory Status", RawValue = inventoryStatus });

			inventoryStatusView.ChildViews.Add(view);
		}

		protected virtual bool GetPublishedStatus(SellableItem sellableItem)
		{
			return sellableItem.Published;
		}
		
		protected virtual bool GetEnabledStatus(SellableItem sellableItem, string variantId)
		{
			if (sellableItem.IsBundle)
			{
				// TODO: Implement support for bundles
				return true;
			}

			if (string.IsNullOrWhiteSpace(variantId))
			{
				if (!sellableItem.HasComponent<ItemVariationsComponent>())
				{
					return true;
				}
				
				var variants = sellableItem.GetComponent<ItemVariationsComponent>().GetComponents<ItemVariationComponent>();
				foreach (var variant in variants)
				{
					if (!variant.Disabled)
					{
						return true;
					}
				}
			}
			else
			{
				var variant = sellableItem.GetVariation(variantId);
				return !variant.Disabled;
			}

			return false;
		}

		protected virtual bool GetPricingStatus(SellableItem sellableItem, string variantId)
		{
			if (string.IsNullOrWhiteSpace(variantId))
			{
				return sellableItem.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice != null;
			}
			else
			{
				var variant = sellableItem.GetVariation(variantId);
				return variant.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice != null;
			}
		}
		
		protected virtual bool ToInventoryAvailability(string inventoryStatus)
		{
			return !inventoryStatus.Equals(Engine.CatalogConstants.InventoryStatus.NotAvailable) && !inventoryStatus.Equals(Engine.CatalogConstants.InventoryStatus.OutOfStock);
		}

		protected async virtual Task<List<SellableItem>> GetSellableItemPerCurrency(string catalogName, string sellableItemId, string variantId, CommercePipelineExecutionContext context)
		{
			var sellableItems = new List<SellableItem>();

			var currencySet = await Commander.Command<GetCurrencySetCommand>().Process(context.CommerceContext, context.GetPolicy<GlobalCurrencyPolicy>().DefaultCurrencySet).ConfigureAwait(false);
			if (!currencySet.HasComponent<CurrenciesComponent>())
			{
				return sellableItems;
			}
			
			var productArgument = new ProductArgument(catalogName, sellableItemId);
			if (!string.IsNullOrWhiteSpace(variantId))
			{
				productArgument.VariantId = variantId;
			}

			var currencies = currencySet.GetComponent<CurrenciesComponent>().Currencies;
			var initialCurrency = context.CommerceContext.Headers["Currency"];
			foreach (var currency in currencies)
			{
				var commerceContext = new CommerceContext(context.CommerceContext.Logger, context.CommerceContext.TelemetryClient)
				{
					Environment = context.CommerceContext.Environment,
					GlobalEnvironment = context.CommerceContext.GlobalEnvironment,
					Headers = context.CommerceContext.Headers
				};
				commerceContext.Headers["Currency"] = currency.Code;
				var sellableItem = await Commander.Command<GetSellableItemCommand>().Process(commerceContext, productArgument.AsItemId(), false).ConfigureAwait(false);
				if (sellableItem != null)
				{
					sellableItems.Add(sellableItem);
				}
			}
			context.CommerceContext.Headers["Currency"] = initialCurrency;

			return sellableItems;
		}
		
		protected async virtual Task<PricingModel> CreatePricingModel(SellableItem sellableItem, string variantId, CommercePipelineExecutionContext context)
		{
			PricingModel pricingModel = null;

			if (string.IsNullOrWhiteSpace(variantId))
			{
				pricingModel = new PricingModel(sellableItem.ListPrice, sellableItem.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice, sellableItem.GetComponent<MessagesComponent>());
			}
			else
			{
				var variation = sellableItem.GetVariation(variantId);
				if (variation == null)
				{
					return await Task.FromResult(pricingModel).ConfigureAwait(false);
				}

				pricingModel = new PricingModel(variation.ListPrice, variation.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice, variation.GetComponent<MessagesComponent>());
			}

			return await Task.FromResult(pricingModel).ConfigureAwait(false);
		}
		
		protected async virtual Task<string> GetInventoryStatus(string inventorySetName, SellableItem sellableItem, string variantId, CommercePipelineExecutionContext context)
		{
			if (sellableItem.IsBundle)
			{
				// TODO: Implement support for bundle
				return "Not Supported";
			}

			if (sellableItem.HasPolicy<AvailabilityAlwaysPolicy>())
			{
				return Engine.CatalogConstants.InventoryStatus.Perpetual;
			}
			
			if (string.IsNullOrWhiteSpace(variantId))
			{
				var inventoryInformation = await Commander.Command<GetInventoryInformationCommand>().Process(context.CommerceContext, inventorySetName, sellableItem.ProductId).ConfigureAwait(false);
				if (inventoryInformation != null)
				{
					return this.ToInventoryStatus(inventoryInformation);
				}
			}

			if (string.IsNullOrWhiteSpace(variantId))
			{
				var variants = sellableItem.GetComponent<ItemVariationsComponent>().GetComponents<ItemVariationComponent>();
				var allNotAvailable = true;
				foreach (var variant in variants)
				{
					var status = await GetVariantInventoryStatus(variant, inventorySetName, context).ConfigureAwait(false);
					allNotAvailable &= status.Equals(Engine.CatalogConstants.InventoryStatus.NotAvailable, StringComparison.InvariantCultureIgnoreCase);
					if (status.Equals(Engine.CatalogConstants.InventoryStatus.NotAvailable, StringComparison.InvariantCultureIgnoreCase)
						|| status.Equals(Engine.CatalogConstants.InventoryStatus.OutOfStock, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}

					return status;
				}

				return allNotAvailable ? Engine.CatalogConstants.InventoryStatus.NotAvailable : Engine.CatalogConstants.InventoryStatus.OutOfStock;
			}
			else
			{
				var variant = sellableItem.GetVariation(variantId);
				return await GetVariantInventoryStatus(variant, inventorySetName, context).ConfigureAwait(false);
			}
			
		}

		protected async virtual Task<string> GetVariantInventoryStatus(ItemVariationComponent variant, string inventorySetName, CommercePipelineExecutionContext context)
		{
			if (variant.HasPolicy<AvailabilityAlwaysPolicy>())
			{
				return Engine.CatalogConstants.InventoryStatus.Perpetual;
			}

			if (!variant.HasComponent<InventoryComponent>())
			{
				return Engine.CatalogConstants.InventoryStatus.NotAvailable;
			}

			var inventoryComponent = variant.GetComponent<InventoryComponent>();
			var association = inventoryComponent.InventoryAssociations.FirstOrDefault(
				i => i.InventorySet.EntityTarget.RemoveIdPrefix<InventorySet>().Equals(inventorySetName, StringComparison.InvariantCultureIgnoreCase));
			if (association != null)
			{
				var inventoryInformation = await Commander.GetEntity<InventoryInformation>(context.CommerceContext, association.InventoryInformation.EntityTarget).ConfigureAwait(false);
				return this.ToInventoryStatus(inventoryInformation);
			}

			return Engine.CatalogConstants.InventoryStatus.NotAvailable;
		}
		
		protected virtual string ToInventoryStatus(InventoryInformation inventoryInformation)
		{
			if (inventoryInformation.Quantity > 0)
			{
				return Engine.CatalogConstants.InventoryStatus.InStock;
			}

			if (inventoryInformation.HasComponent<PreorderableComponent>())
			{
				var component = inventoryInformation.GetComponent<PreorderableComponent>();
				if (component.Preorderable && component.PreorderLimit > 0 && component.PreorderedQuantity < component.PreorderLimit)
				{
					return Engine.CatalogConstants.InventoryStatus.OnPreOrder;
				}
			}

			if (inventoryInformation.HasComponent<BackorderableComponent>())
			{
				var component = inventoryInformation.GetComponent<BackorderableComponent>();
				if (component.Backorderable && component.BackorderLimit > 0 && component.BackorderedQuantity < component.BackorderLimit)
				{
					return Engine.CatalogConstants.InventoryStatus.OnBackOrder;
				}
			}

			return Engine.CatalogConstants.InventoryStatus.OutOfStock;
		}
	}
}
