using Sitecore.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ajsuth.Foundation.Catalog.Engine.Extensions
{
	public static class MessagesExtensions
	{
		public static IEnumerable<MessageModel> GetListPriceMessages(this MessagesComponent messagesComponent)
		{
			return messagesComponent.Messages.Where(m => m.Code.Equals("Pricing", StringComparison.InvariantCultureIgnoreCase) && (m.Text.StartsWith("ListPrice") || m.Text.StartsWith("Variation.ListPrice")));
		}

		public static IEnumerable<MessageModel> GetSellPriceMessages(this MessagesComponent messagesComponent)
		{
			return messagesComponent.Messages.Where(m => m.Code.Equals("Pricing", StringComparison.InvariantCultureIgnoreCase) && (m.Text.StartsWith("SellPrice") || m.Text.StartsWith("Variation.SellPrice")));
		}
	}
}
