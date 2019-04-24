using Ajsuth.Foundation.Catalog.Engine.Extensions;
using Sitecore.Commerce.Core;
using System.Collections.Generic;
using System.Linq;

namespace Ajsuth.Foundation.Catalog.Engine.Models
{
    public class PricingModel : Model
	{
		public PricingModel()
		{
			ListPriceMessages = new List<string>();
			SellPriceMessages = new List<string>();
		}

		public PricingModel(Money listPrice, Money sellPrice, MessagesComponent messagesComponent) : this()
		{
			ListPrice = listPrice;
			SellPrice = sellPrice;

			var listPriceMessages = messagesComponent.GetListPriceMessages().Select(m => m.Text);
			if (listPriceMessages != null)
			{
				ListPriceMessages.AddRange(listPriceMessages);
			}

			var sellPriceMessages = messagesComponent.GetSellPriceMessages().Select(m => m.Text);
			if (sellPriceMessages != null)
			{
				SellPriceMessages.AddRange(sellPriceMessages);
			}
		}

		public Money ListPrice { get; set; }
		public Money SellPrice { get; set; }
		public List<string> ListPriceMessages { get; set; }
		public List<string> SellPriceMessages { get; set; }
	}
}
