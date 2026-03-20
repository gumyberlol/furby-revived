using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class GeoCodeLocking
	{
		public enum GeoCodeOperator
		{
			Exclusion = 10,
			Inclusion = 20
		}

		[SerializeField]
		public GeoCodeOperator m_Operator = GeoCodeOperator.Inclusion;

		[SerializeField]
		public CountryCode_Using_ISO3166_2[] m_FilterSet = new CountryCode_Using_ISO3166_2[0];

		public static bool IsPermitted(string geoCode, GeoCodeLocking geoCodeLocking)
		{
			GeoCodeOperator geoCodeOperator = geoCodeLocking.m_Operator;
			CountryCode_Using_ISO3166_2[] filterSet = geoCodeLocking.m_FilterSet;
			bool result = false;
			switch (geoCodeOperator)
			{
			case GeoCodeOperator.Exclusion:
			{
				result = true;
				CountryCode_Using_ISO3166_2[] array2 = filterSet;
				foreach (CountryCode_Using_ISO3166_2 countryCode_Using_ISO3166_2 in array2)
				{
					if (geoCode.ToString().StartsWith(countryCode_Using_ISO3166_2.ToString()))
					{
						result = false;
						break;
					}
				}
				break;
			}
			case GeoCodeOperator.Inclusion:
			{
				result = false;
				CountryCode_Using_ISO3166_2[] array = filterSet;
				foreach (CountryCode_Using_ISO3166_2 countryCode_Using_ISO3166_ in array)
				{
					if (geoCode.ToString().StartsWith(countryCode_Using_ISO3166_.ToString()))
					{
						result = true;
						break;
					}
				}
				break;
			}
			}
			return result;
		}
	}
}
