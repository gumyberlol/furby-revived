using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class AdultFurbyLibrary : RelentlessMonoBehaviour
	{
		private Dictionary<AdultFurbyType, FurbyData> m_furbyDictionary = new Dictionary<AdultFurbyType, FurbyData>();

		[SerializeField]
		private FurbyData[] m_furbies;

		[SerializeField]
		private UnlockLevels m_unlockLevels;

		[SerializeField]
		private UnlockLevels m_xpLevels;

		[SerializeField]
		private UnlockLevels m_eggCost;

		[SerializeField]
		private FurbyData m_noFurbyData;

		[SerializeField]
		public int m_LastIndex_MainTribes;

		[SerializeField]
		public int m_LastIndex_Spring;

		[SerializeField]
		public int m_LastIndex_Crystal;

		public IList<FurbyData> Furbies
		{
			get
			{
				if (Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal)
				{
					return GetFurbyDataToDepth(0, m_LastIndex_Crystal + 1);
				}
				if (Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring)
				{
					return GetFurbyDataToDepth(0, m_LastIndex_Spring + 1);
				}
				return GetFurbyDataToDepth(0, m_LastIndex_MainTribes + 1);
			}
		}

		public IList<int> UnlockLevels
		{
			get
			{
				return m_unlockLevels.GetUnlockLevels();
			}
		}

		public IList<int> XpLevels
		{
			get
			{
				return m_xpLevels.Levels;
			}
		}

		public IList<int> EggCost
		{
			get
			{
				return m_eggCost.GetUnlockLevels();
			}
		}

		public FurbyData[] GetFurbyDataToDepth(int index, int length)
		{
			FurbyData[] array = new FurbyData[length];
			Array.Copy(m_furbies, index, array, 0, length);
			return array;
		}

		private void Awake()
		{
			BuildDictionary();
		}

		public void ClearDictionary()
		{
			if (m_furbyDictionary.Count > 0)
			{
				Logging.Log("BabyFurbyLibrary [" + base.name + "] Contains [" + m_furbyDictionary.Count + "] Furblings -> DELETING!");
			}
			m_furbyDictionary.Clear();
		}

		public void BuildDictionary()
		{
			m_furbyDictionary.Clear();
			foreach (FurbyData furby in Furbies)
			{
				m_furbyDictionary.Add(furby.AdultType, furby);
			}
			string text = "AdultFurbyLibrary [" + base.name + "] Contains [" + m_furbyDictionary.Count + "] Adults\nThe list follows...\n";
			foreach (KeyValuePair<AdultFurbyType, FurbyData> item in m_furbyDictionary)
			{
				text = text + item.ToString() + "\n";
			}
			Logging.Log(text);
		}

		public static AdultFurbyType GetAdultFurbyTypeFromToy()
		{
			return ConvertComAirPatternToAdultType(Singleton<FurbyDataChannel>.Instance.FurbyStatus.Pattern);
		}

		public static AdultFurbyType ConvertComAirPatternToAdultType(FurbyPattern pattern)
		{
			switch (pattern)
			{
			case FurbyPattern.Checkerboard:
				return AdultFurbyType.Checkerboard;
			case FurbyPattern.Christmas:
				return AdultFurbyType.Christmas;
			case FurbyPattern.Cubes:
				return AdultFurbyType.Cubes;
			case FurbyPattern.Diagonals:
				return AdultFurbyType.Diagonals;
			case FurbyPattern.Diamonds:
				return AdultFurbyType.Diamonds;
			case FurbyPattern.Hearts:
				return AdultFurbyType.Hearts;
			case FurbyPattern.Lightning:
				return AdultFurbyType.Lightning;
			case FurbyPattern.Peacock:
				return AdultFurbyType.Peacock;
			case FurbyPattern.Triangles:
				return AdultFurbyType.Triangles;
			case FurbyPattern.Waves:
				return AdultFurbyType.Waves;
			case FurbyPattern.Zigzags:
				return AdultFurbyType.Zigzags;
			case FurbyPattern.Stripes:
				return AdultFurbyType.Stripes;
			case FurbyPattern.SpringHeart:
				return AdultFurbyType.SpringHearts;
			case FurbyPattern.SpringStar:
				return AdultFurbyType.SpringStar;
			case FurbyPattern.SpringDiamonds:
				return AdultFurbyType.SpringDiamond;
			case FurbyPattern.SpringWaves:
				return AdultFurbyType.SpringRainbows;
			case FurbyPattern.SpringHoundstooth:
				return AdultFurbyType.SpringHoundstooth;
			case FurbyPattern.SpringEaster:
				return AdultFurbyType.SpringZigZag;
			case FurbyPattern.CrystalYellowToOrange:
				return AdultFurbyType.CrystalYellowToOrange;
			case FurbyPattern.CrystalOrangeToPink:
				return AdultFurbyType.CrystalOrangeToPink;
			case FurbyPattern.CrystalPinkToPurple:
				return AdultFurbyType.CrystalPinkToPurple;
			case FurbyPattern.CrystalGreenToBlue:
				return AdultFurbyType.CrystalGreenToBlue;
			case FurbyPattern.CrystalPinkToBlue:
				return AdultFurbyType.CrystalPinkToBlue;
			case FurbyPattern.CrystalRainbow:
				return AdultFurbyType.CrystalRainbow;
			default:
				return AdultFurbyType.Zigzags;
			}
		}

		public FurbyData GetAdultFurby(AdultFurbyType type)
		{
			return m_furbyDictionary[type];
		}
	}
}
