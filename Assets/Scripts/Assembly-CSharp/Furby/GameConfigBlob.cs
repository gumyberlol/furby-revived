using System;
using Furby.Incubator;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class GameConfigBlob
	{
		[SerializeField]
		public IncubatorTimings m_IncubatorTimings;

		[SerializeField]
		public IncubatorTimings m_FurbyIncubatorTimings;

		[SerializeField]
		public IncubatorTimings m_IncubatorTimings_FF;

		[SerializeField]
		public GameConsumable[] m_IncubatorConsumables = new GameConsumable[0];

		[SerializeField]
		public string m_CrystalLookBundleID = string.Empty;

		[SerializeField]
		public bool m_EnableLongerIncubationTimes;

		[SerializeField]
		public int m_FastForwardPurchaseUnits = 10;

		[SerializeField]
		public int m_FastForwardAvailabilityThreshold = 1;

		[SerializeField]
		public bool m_RequireValidatedReceipts = true;

		[SerializeField]
		public GeoCodeLocking m_InAppProductsLocking = new GeoCodeLocking();

		[SerializeField]
		public GeoCodeLocking m_SpringTribeLockling = new GeoCodeLocking();

		[SerializeField]
		public GeoCodeLocking m_CrystalTribeLocking = new GeoCodeLocking();

		[SerializeField]
		public BannerAdvert m_Banner320x50 = new BannerAdvert();

		[SerializeField]
		public BannerAdvert m_Banner768x66 = new BannerAdvert();

		[SerializeField]
		public GeoCodeLocking m_BannerAdvertLocking = new GeoCodeLocking();

		[SerializeField]
		public bool m_SuppressAllBanners = true;

		public bool SuppressAllBanners
		{
			get
			{
				return m_SuppressAllBanners;
			}
		}

		public bool DoesGeoCodeAllowTribe(Tribeset tribe)
		{
			string countryCode = Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode;
			bool result = false;
			switch (tribe)
			{
			case Tribeset.MainTribes:
			case Tribeset.Promo:
			case Tribeset.Golden:
				return true;
			case Tribeset.Spring:
				result = GeoCodeLocking.IsPermitted(countryCode, m_SpringTribeLockling);
				break;
			case Tribeset.CrystalGem:
				result = GeoCodeLocking.IsPermitted(countryCode, m_CrystalTribeLocking);
				break;
			}
			return result;
		}

		public bool DoesGeoCodeAllowIAP(string geoCode)
		{
			return GeoCodeLocking.IsPermitted(geoCode, m_InAppProductsLocking);
		}

		public bool DoesGeoCodeAllowBannerAdverts()
		{
			string countryCode = Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode;
			return GeoCodeLocking.IsPermitted(countryCode, m_BannerAdvertLocking);
		}
	}
}
