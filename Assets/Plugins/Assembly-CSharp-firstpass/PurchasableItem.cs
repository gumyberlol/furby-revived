using System;
using System.Collections.Generic;
using Unibill.Impl;

public class PurchasableItem : IEquatable<PurchasableItem>
{
	internal class Writer
	{
		public static void setLocalizedPrice(PurchasableItem item, decimal price)
		{
			item.localizedPrice = price;
			item.localizedPriceString = price.ToString();
		}

		public static void setLocalizedPrice(PurchasableItem item, string price)
		{
			item.localizedPriceString = price;
		}

		public static void setLocalizedTitle(PurchasableItem item, string title)
		{
			item.localizedTitle = title;
		}

		public static void setLocalizedDescription(PurchasableItem item, string description)
		{
			item.localizedDescription = description;
		}

		public static void setPriceInLocalCurrency(PurchasableItem item, decimal amount)
		{
			item.priceInLocalCurrency = amount;
		}

		public static void setISOCurrencySymbol(PurchasableItem item, string code)
		{
			item.isoCurrencySymbol = code;
		}
	}

	public Dictionary<BillingPlatform, Dictionary<string, object>> platformBundles;

	private BillingPlatform platform;

	public string Id { get; set; }

	public PurchaseType PurchaseType { get; set; }

	public string name { get; set; }

	public string description { get; set; }

	public decimal localizedPrice { get; private set; }

	public string localizedPriceString { get; private set; }

	public string localizedTitle { get; private set; }

	public string localizedDescription { get; private set; }

	public string isoCurrencySymbol { get; private set; }

	public decimal priceInLocalCurrency { get; private set; }

	public string LocalId
	{
		get
		{
			if (string.IsNullOrEmpty(LocalIds[platform]))
			{
				return Id;
			}
			return LocalIds[platform];
		}
	}

	public Dictionary<BillingPlatform, string> LocalIds { get; private set; }

	public PurchasableItem()
	{
		Id = new Random().Next().ToString();
		description = "Describe me!";
		name = "Name me!";
		PurchaseType = PurchaseType.NonConsumable;
		platformBundles = new Dictionary<BillingPlatform, Dictionary<string, object>>();
		LocalIds = new Dictionary<BillingPlatform, string>();
		foreach (int value in Enum.GetValues(typeof(BillingPlatform)))
		{
			platformBundles[(BillingPlatform)value] = new Dictionary<string, object>();
			LocalIds[(BillingPlatform)value] = string.Empty;
		}
	}

	public PurchasableItem(string id, Dictionary<string, object> hash, BillingPlatform platform)
	{
		Id = id;
		this.platform = platform;
		Deserialize(hash);
	}

	private void Deserialize(Dictionary<string, object> hash)
	{
		PurchaseType = hash.getEnum<PurchaseType>("@purchaseType");
		name = hash.get<string>("name");
		description = hash.get<string>("description");
		localizedTitle = name;
		localizedDescription = description;
		priceInLocalCurrency = 1m;
		isoCurrencySymbol = "USD";
		LocalIds = new Dictionary<BillingPlatform, string>();
		platformBundles = new Dictionary<BillingPlatform, Dictionary<string, object>>();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)hash["platforms"];
		foreach (int value in Enum.GetValues(typeof(BillingPlatform)))
		{
			if (dictionary.ContainsKey(((BillingPlatform)value).ToString()))
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary[((BillingPlatform)value).ToString()];
				string key = string.Format("{0}.Id", (BillingPlatform)value);
				if (dictionary2 != null && dictionary2.ContainsKey(key))
				{
					LocalIds.Add((BillingPlatform)value, (string)dictionary2[key]);
				}
				if (dictionary2 != null)
				{
					platformBundles[(BillingPlatform)value] = dictionary2;
				}
			}
			if (!LocalIds.ContainsKey((BillingPlatform)value))
			{
				LocalIds[(BillingPlatform)value] = Id;
			}
			if (!platformBundles.ContainsKey((BillingPlatform)value))
			{
				platformBundles[(BillingPlatform)value] = new Dictionary<string, object>();
			}
		}
	}

	public Dictionary<string, object> Serialize()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("@id", Id);
		dictionary.Add("@purchaseType", PurchaseType.ToString());
		dictionary.Add("name", name);
		dictionary.Add("description", description);
		dictionary.Add("platforms", platformBundles);
		return dictionary;
	}

	public bool Equals(PurchasableItem other)
	{
		return other.Id == Id;
	}
}
