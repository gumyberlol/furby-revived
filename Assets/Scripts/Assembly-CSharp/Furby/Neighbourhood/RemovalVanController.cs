using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class RemovalVanController : RelentlessMonoBehaviour
	{
		public AnimationClip m_animTowerPromo;

		public AnimationClip[] m_TowerAnimations;

		public HoodController m_HoodController_Main;

		public HoodController m_HoodController_Golden;

		[HideInInspector]
		private HoodController m_ActiveHoodController;

		public Transform m_vfxRoot;

		public GameObject[] m_objectsToDisable;

		public UICamera[] m_uiCamerasToDisable;

		private bool m_ReenableUIOnCompletion = true;

		public void DontReenableUIOnCompletion()
		{
			m_ReenableUIOnCompletion = false;
		}

		public void PrepareForRemoval(FurbyBaby furbyBaby)
		{
			m_ReenableUIOnCompletion = true;
			DisableUI();
			switch (furbyBaby.Tribe.TribeSet)
			{
			case Tribeset.Golden:
			case Tribeset.CrystalGolden:
				m_ActiveHoodController = m_HoodController_Golden;
				break;
			case Tribeset.MainTribes:
			case Tribeset.Promo:
			case Tribeset.Spring:
			case Tribeset.CrystalGem:
				m_ActiveHoodController = m_HoodController_Main;
				break;
			}
			m_ActiveHoodController.m_TheFurblingInFocus = furbyBaby;
			PrepareRemovalVan();
		}

		public void PrepareRemovalVan()
		{
			if (m_vfxRoot != null)
			{
				m_vfxRoot.position = m_ActiveHoodController.m_newArrivalFurblingWindow.transform.position;
			}
		}

		public void SendRemovalVanToTower(int towerIndex, FurbyBaby furbyBaby)
		{
			base.animation.clip = m_TowerAnimations[towerIndex];
			base.animation.Play();
		}

		public void SendRemovalVanToCenter()
		{
			base.animation.clip = m_animTowerPromo;
			base.animation.Play();
		}

		public void TellTheRemovalVanControllerToDeliverFurbling()
		{
			DebugUtils.Log_InOrange("TellTheRemovalVanControllerToDeliverFurbling()");
			DeliverFurbling();
		}

		public void DeliverFurbling()
		{
			DebugUtils.Log_InOrange("DeliverFurbling()");
			if (m_ActiveHoodController.m_NeighbourhoodTribeSet == Tribeset.CrystalGem)
			{
				GameEventRouter.SendEvent(HoodEvents.Hood_NewResident_PlacedFully);
			}
			m_ActiveHoodController.EnableWindow(m_ActiveHoodController.m_newArrivalFurblingWindow);
			m_ActiveHoodController.ReDoFlags();
			switch (m_ActiveHoodController.m_NeighbourhoodTribeSet)
			{
			case Tribeset.Golden:
				GameEventRouter.SendEvent(HoodEvents.Hood_GoldFurblingWon);
				break;
			case Tribeset.CrystalGolden:
				GameEventRouter.SendEvent(HoodEvents.Hood_CrystalUFO_Leaves);
				GameEventRouter.SendEvent(HoodEvents.Hood_CrystalGemFurblingWon);
				break;
			case Tribeset.CrystalGem:
				GameEventRouter.SendEvent(HoodEvents.Hood_CrystalUFO_Leaves);
				break;
			}
			if (m_ReenableUIOnCompletion)
			{
				Invoke(ReEnableUI, 2f);
			}
		}

		public void DisableUI()
		{
			GameObject[] objectsToDisable = m_objectsToDisable;
			foreach (GameObject gameObject in objectsToDisable)
			{
				gameObject.SetActive(false);
			}
			UICamera[] uiCamerasToDisable = m_uiCamerasToDisable;
			foreach (UICamera uICamera in uiCamerasToDisable)
			{
				if (uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = false;
				}
			}
		}

		public void ReEnableUI()
		{
			GameObject[] objectsToDisable = m_objectsToDisable;
			foreach (GameObject gameObject in objectsToDisable)
			{
				gameObject.SetActive(true);
			}
			UICamera[] uiCamerasToDisable = m_uiCamerasToDisable;
			foreach (UICamera uICamera in uiCamerasToDisable)
			{
				if (uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = true;
				}
			}
		}
	}
}
