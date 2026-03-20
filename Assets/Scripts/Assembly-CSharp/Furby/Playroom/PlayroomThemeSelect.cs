using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomThemeSelect : InGamePurchaseableItem
	{
		public SelectableTheme m_SelectableTheme;

		public bool m_IsTop;

		public PlayroomHintController m_HintController;

		public override void OnClickAlreadyPurchased()
		{
			SelectTheme();
			GameEventRouter.SendEvent(PlayroomGameEvent.Customization_ItemChanged);
			if (Singleton<PlayroomCustomizationModeController>.Instance.CloseCarouselOnSelection)
			{
				Singleton<PlayroomCustomizationModeController>.Instance.RefreshCustomizationMode(PlayroomCustomizationMode.None);
			}
		}

		public void OnDrag(Vector2 delta)
		{
			if (m_IsTop)
			{
				m_HintController.ScrollTop.Disable();
			}
			else
			{
				m_HintController.ScrollBot.Disable();
			}
		}

		private void Start()
		{
			GameObject gameObject = GameObject.Find("HintController");
			m_HintController = gameObject.GetComponent<PlayroomHintController>();
			SetTickAppropriately();
			SetGrayedOutAppropriately();
			EnableAppropriateHintController();
		}

		private void OnDisable()
		{
			if ((bool)m_HintController)
			{
				m_HintController.ScrollTop.Disable();
				m_HintController.ScrollBot.Disable();
				m_HintController.SelectItemTop.Disable();
				m_HintController.SelectItemBot.Disable();
			}
		}

		private void EnableAppropriateHintController()
		{
			GameObject gameObject = GameObject.Find("HintController");
			m_HintController = gameObject.GetComponent<PlayroomHintController>();
			if (m_IsTop)
			{
				m_HintController.ScrollTop.Enable();
				m_HintController.SelectItemTop.Enable();
				m_HintController.ScrollBot.Disable();
				m_HintController.SelectItemBot.Disable();
			}
			else
			{
				m_HintController.ScrollTop.Disable();
				m_HintController.SelectItemTop.Disable();
				m_HintController.ScrollBot.Enable();
				m_HintController.SelectItemBot.Enable();
			}
		}

		private void SetGrayedOutAppropriately()
		{
			SetLocked(!WholeGameShopHelpers.IsItemUnlocked(m_SelectableTheme));
		}

		private void SetTickAppropriately()
		{
			if (HasThemeBeenApplied())
			{
				base.gameObject.GetChildGameObject("GUI_Tick").SetActive(true);
			}
		}

		private bool HasThemeBeenApplied()
		{
			GameObject gameObject = GameObject.Find("DefaultItems");
			DefaultRoomItems component = gameObject.GetComponent<DefaultRoomItems>();
			Texture wallTexture = m_SelectableTheme.m_PlayroomThemeData.m_WallTexture;
			Texture wallTexture2 = component.m_CurrentItems.m_Theme.m_WallTexture;
			if (wallTexture == wallTexture2)
			{
				return true;
			}
			return false;
		}

		private void SelectTheme()
		{
			DeselectOtherThemes();
			ApplyTheme();
			base.gameObject.GetChildGameObject("GUI_Tick").SetActive(true);
			SavePreference();
		}

		private void SavePreference()
		{
			BabyInstance babyInstance = (BabyInstance)Object.FindObjectOfType(typeof(BabyInstance));
			if (!(babyInstance == null))
			{
				FurbyBaby targetFurbyBaby = babyInstance.GetTargetFurbyBaby();
				targetFurbyBaby.PlayroomCustomizations.Theme = m_SelectableTheme.GetName();
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		public void DeselectOtherThemes()
		{
			foreach (Transform item in base.transform.parent.gameObject.transform)
			{
				GameObject childGameObject = item.gameObject.GetChildGameObject("GUI_Tick");
				if ((bool)childGameObject && childGameObject.activeSelf)
				{
					childGameObject.SetActive(false);
				}
			}
		}

		public void SetThemeData(PlayroomThemeData playroomThemeData)
		{
			m_SelectableTheme.m_PlayroomThemeData = playroomThemeData;
		}

		public void ApplyTheme()
		{
			m_SelectableTheme.m_PlayroomThemeData.m_WallMaterial.SetTexture("_MainTex", m_SelectableTheme.m_PlayroomThemeData.m_WallTexture);
			m_SelectableTheme.m_PlayroomThemeData.m_InteriorMaterial.SetTexture("_MainTex", m_SelectableTheme.m_PlayroomThemeData.m_InteriorTexture);
			m_SelectableTheme.m_PlayroomThemeData.m_WallMaterial.SetTexture("_Texture", m_SelectableTheme.m_PlayroomThemeData.m_WallTexture);
			m_SelectableTheme.m_PlayroomThemeData.m_InteriorMaterial.SetTexture("_Texture", m_SelectableTheme.m_PlayroomThemeData.m_InteriorTexture);
		}

		public override int GetFurbucksCost()
		{
			return m_SelectableTheme.m_PlayroomThemeData.m_cost;
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_SelectableTheme.m_PlayroomThemeData.m_Name);
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_SelectableTheme);
		}

		public override bool ShouldUseAfterPurchase()
		{
			return true;
		}
	}
}
