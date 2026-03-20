using Furby.Playroom;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class GiftPresentation : MonoBehaviour
	{
		[SerializeField]
		public SelectableFeatureList[] m_PlayroomFeatureLists;

		[SerializeField]
		public SelectableThemeList[] m_PlayroomThemeLists;

		public string GetGiftDisplayName(int giftIndex)
		{
			GiftItem giftItemAtIndex = GetGiftItemAtIndex(giftIndex);
			switch (giftItemAtIndex.m_GiftType)
			{
			case GiftItemType.PlayroomFeature:
				return GetPlayroomFeatureMetadata(giftItemAtIndex.m_GiftID).m_Name;
			case GiftItemType.PlayroomTheme:
				return GetPlayroomThemeMetadata(giftItemAtIndex.m_GiftID).m_Name;
			default:
				return null;
			}
		}

		public UIAtlas GetCarouselAtlasForGift(int giftIndex)
		{
			GiftItem giftItemAtIndex = GetGiftItemAtIndex(giftIndex);
			UIAtlas uIAtlas = null;
			switch (giftItemAtIndex.m_GiftType)
			{
			case GiftItemType.PlayroomFeature:
				uIAtlas = GetPlayroomFeatureMetadata(giftItemAtIndex.m_GiftID).m_UIAtlas;
				break;
			case GiftItemType.PlayroomTheme:
				uIAtlas = GetPlayroomThemeMetadata(giftItemAtIndex.m_GiftID).m_UIAtlas;
				break;
			}
			if (uIAtlas == null)
			{
				Logging.LogError("GetCarouselAtlasForGift Can't find gift indexed at: " + giftIndex);
			}
			return uIAtlas;
		}

		public UIAtlas GetUnlockAtlasForGift(int giftIndex)
		{
			GiftItem giftItemAtIndex = GetGiftItemAtIndex(giftIndex);
			UIAtlas uIAtlas = null;
			switch (giftItemAtIndex.m_GiftType)
			{
			case GiftItemType.PlayroomFeature:
				uIAtlas = GetPlayroomFeatureMetadata(giftItemAtIndex.m_GiftID).m_UIAtlas;
				break;
			case GiftItemType.PlayroomTheme:
				uIAtlas = GetPlayroomThemeMetadata(giftItemAtIndex.m_GiftID).m_UIAtlas;
				break;
			}
			if (uIAtlas == null)
			{
				Logging.LogError("GetUnlockAtlasForGift Can't find gift indexed at: " + giftIndex);
			}
			return uIAtlas;
		}

		public string GetCarouselSpriteNameForGift(int giftIndex)
		{
			GiftItem giftItemAtIndex = GetGiftItemAtIndex(giftIndex);
			string text = string.Empty;
			switch (giftItemAtIndex.m_GiftType)
			{
			case GiftItemType.PlayroomFeature:
				text = GetPlayroomFeatureMetadata(giftItemAtIndex.m_GiftID).m_SpriteName;
				break;
			case GiftItemType.PlayroomTheme:
				text = GetPlayroomThemeMetadata(giftItemAtIndex.m_GiftID).m_SpriteName;
				break;
			}
			if (text.Equals(string.Empty))
			{
				Logging.LogError("GetCarouselSpriteNameForGift Can't find gift indexed at: " + giftIndex);
			}
			return text;
		}

		public string GetUnlockSpriteNameForGift(int giftIndex)
		{
			GiftItem giftItemAtIndex = GetGiftItemAtIndex(giftIndex);
			string text = string.Empty;
			switch (giftItemAtIndex.m_GiftType)
			{
			case GiftItemType.PlayroomFeature:
				text = GetPlayroomFeatureMetadata(giftItemAtIndex.m_GiftID).m_SpriteName;
				break;
			case GiftItemType.PlayroomTheme:
				text = GetPlayroomThemeMetadata(giftItemAtIndex.m_GiftID).m_SpriteName;
				break;
			}
			if (text.Equals(string.Empty))
			{
				Logging.LogError("GetUnlockSpriteNameForGift Can't find gift indexed at: " + giftIndex);
			}
			return text;
		}

		private GiftItem GetGiftItemAtIndex(int giftIndex)
		{
			return FurbyGlobals.GiftList.m_GiftItemData[giftIndex];
		}

		private PlayroomFeatureData GetPlayroomFeatureMetadata(string giftID)
		{
			SelectableFeatureList[] playroomFeatureLists = m_PlayroomFeatureLists;
			foreach (SelectableFeatureList selectableFeatureList in playroomFeatureLists)
			{
				foreach (SelectableFeature feature in selectableFeatureList.Features)
				{
					if (feature.GetName().Equals(giftID))
					{
						return feature.PlayroomFeature;
					}
				}
			}
			Logging.LogError("GiftPresentation:: Couldn't find PlayroomFeature Gift: " + giftID);
			return null;
		}

		private PlayroomThemeData GetPlayroomThemeMetadata(string giftID)
		{
			SelectableThemeList[] playroomThemeLists = m_PlayroomThemeLists;
			foreach (SelectableThemeList selectableThemeList in playroomThemeLists)
			{
				foreach (SelectableTheme theme in selectableThemeList.Themes)
				{
					if (theme.GetName().Equals(giftID))
					{
						return theme.PlayroomThemeData;
					}
				}
			}
			Logging.LogError("GiftPresentation:: Couldn't find PlayroomTheme Gift: " + giftID);
			return null;
		}
	}
}
