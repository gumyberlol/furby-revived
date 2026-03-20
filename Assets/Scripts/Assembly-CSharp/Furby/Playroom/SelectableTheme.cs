using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class SelectableTheme : IPlayroomSelectable
	{
		[SerializeField]
		public PlayroomThemeData m_PlayroomThemeData;

		public PlayroomThemeData PlayroomThemeData
		{
			get
			{
				return m_PlayroomThemeData;
			}
			set
			{
				m_PlayroomThemeData = value;
			}
		}

		public SelectableTheme()
		{
			m_PlayroomThemeData = new PlayroomThemeData();
		}

		public SelectableTheme(UIAtlas atlas, Material wallMaterial, Material interiorMaterial)
		{
			m_PlayroomThemeData = new PlayroomThemeData();
			m_PlayroomThemeData.m_UIAtlas = atlas;
			m_PlayroomThemeData.m_WallMaterial = wallMaterial;
			m_PlayroomThemeData.m_InteriorMaterial = interiorMaterial;
		}

		public void AddToScene()
		{
			m_PlayroomThemeData.m_WallMaterial.SetTexture("_MainTex", m_PlayroomThemeData.m_WallTexture);
			m_PlayroomThemeData.m_InteriorMaterial.SetTexture("_MainTex", m_PlayroomThemeData.m_InteriorTexture);
			m_PlayroomThemeData.m_WallMaterial.SetTexture("_Texture", m_PlayroomThemeData.m_WallTexture);
			m_PlayroomThemeData.m_InteriorMaterial.SetTexture("_Texture", m_PlayroomThemeData.m_InteriorTexture);
		}

		public void RemoveFromScene()
		{
		}

		public int GetCost()
		{
			return m_PlayroomThemeData.m_cost;
		}

		public string GetName()
		{
			return m_PlayroomThemeData.m_Name;
		}

		public UIAtlas GetUIAtlas()
		{
			return m_PlayroomThemeData.m_UIAtlas;
		}

		public string GetSpriteName()
		{
			return m_PlayroomThemeData.m_SpriteName;
		}

		public bool IsGoldenItem()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedByGoldenFurby;
		}

		public bool IsGoldenItemOrComAirTone()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedByGoldenFurbyOrComAir;
		}

		public bool IsGoldenCrystalItem()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedAsGoldenCrystal;
		}

		public bool IsAvailableFromTheStart()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.AvailableFromTheStart;
		}

		public bool IsAvailableForPurchase()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.BuyableInShop;
		}

		public bool IsUnlockedByScannedQRCode()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedByScannedQRCode;
		}

		public bool IsUnlockedByComAirTone()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedByComAirTone;
		}

		public bool IsUnlockedBySeason()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedAsSeasonalTheme;
		}

		public bool IsUnlockedByCrystal()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedAsCrystal;
		}

		public bool IsUnlockedAsGift()
		{
			return m_PlayroomThemeData.m_State == PlayroomItemState.UnlockedAsGift;
		}

		public string GetQRCode()
		{
			return m_PlayroomThemeData.m_UnlockCode;
		}

		public string GetVariantCode()
		{
			return m_PlayroomThemeData.m_VariantCode;
		}

		public int GetComAirTone()
		{
			return m_PlayroomThemeData.m_ComAirTone;
		}

		public bool IsTheActualSeasonUnlocked()
		{
			return (bool)m_PlayroomThemeData.m_ThemePeriod && m_PlayroomThemeData.m_ThemePeriod.IsUnlockedNow();
		}
	}
}
