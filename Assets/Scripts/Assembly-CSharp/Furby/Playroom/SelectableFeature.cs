using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class SelectableFeature : IPlayroomSelectable
	{
		[SerializeField]
		public PlayroomFeatureData m_PlayroomFeature;

		public PlayroomFeatureData PlayroomFeature
		{
			get
			{
				return m_PlayroomFeature;
			}
			set
			{
				m_PlayroomFeature = value;
			}
		}

		public SelectableFeature()
		{
			m_PlayroomFeature = new PlayroomFeatureData();
		}

		public SelectableFeature(UIAtlas atlas)
		{
			m_PlayroomFeature = new PlayroomFeatureData();
			m_PlayroomFeature.m_UIAtlas = atlas;
		}

		public void AddToScene()
		{
		}

		public void RemoveFromScene()
		{
		}

		public int GetCost()
		{
			return m_PlayroomFeature.m_Cost;
		}

		public string GetName()
		{
			return m_PlayroomFeature.m_Name;
		}

		public UIAtlas GetUIAtlas()
		{
			return m_PlayroomFeature.m_UIAtlas;
		}

		public string GetSpriteName()
		{
			return m_PlayroomFeature.m_SpriteName;
		}

		public bool IsGoldenItem()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedByGoldenFurby;
		}

		public bool IsGoldenItemOrComAirTone()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedByGoldenFurbyOrComAir;
		}

		public bool IsGoldenCrystalItem()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedAsGoldenCrystal;
		}

		public bool IsAvailableFromTheStart()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.AvailableFromTheStart;
		}

		public bool IsAvailableForPurchase()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.BuyableInShop;
		}

		public bool IsUnlockedByScannedQRCode()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedByScannedQRCode;
		}

		public bool IsUnlockedByComAirTone()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedByComAirTone;
		}

		public bool IsUnlockedBySeason()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedAsSeasonalTheme;
		}

		public bool IsUnlockedByCrystal()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedAsCrystal;
		}

		public bool IsUnlockedAsGift()
		{
			return m_PlayroomFeature.m_State == PlayroomItemState.UnlockedAsGift;
		}

		public string GetQRCode()
		{
			return m_PlayroomFeature.m_UnlockCode;
		}

		public string GetVariantCode()
		{
			return m_PlayroomFeature.m_VariantCode;
		}

		public string GetQRCodeWithVariant()
		{
			return m_PlayroomFeature.m_UnlockCode + m_PlayroomFeature.m_VariantCode;
		}

		public int GetComAirTone()
		{
			return m_PlayroomFeature.m_ComAirTone;
		}

		public bool IsTheActualSeasonUnlocked()
		{
			return (bool)m_PlayroomFeature.m_ThemePeriod && m_PlayroomFeature.m_ThemePeriod.IsUnlockedNow();
		}
	}
}
