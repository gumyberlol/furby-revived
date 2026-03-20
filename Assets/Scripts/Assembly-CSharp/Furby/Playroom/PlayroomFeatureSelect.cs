using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomFeatureSelect : InGamePurchaseableItem
	{
		public SelectableFeature m_Feature;

		public string m_TargetAssetBundle;

		public GameObject m_TargetRoot;

		private GameObject m_InstancedItem;

		public bool m_IsTop;

		public PlayroomHintController m_HintController;

		private static bool m_AmSwitchingPlayroomFeature;

		public void SetFeatureData(PlayroomFeatureData featuredata)
		{
			m_Feature.m_PlayroomFeature = featuredata;
		}

		public override void OnClickAlreadyPurchased()
		{
			if (!m_AmSwitchingPlayroomFeature)
			{
				m_AmSwitchingPlayroomFeature = true;
				if (!AssetBundleHelpers.IsLoading())
				{
					StartCoroutine(SelectFeature());
					GameEventRouter.SendEvent(PlayroomGameEvent.Customization_ItemChanged);
					if (Singleton<PlayroomCustomizationModeController>.Instance.CloseCarouselOnSelection)
					{
						Singleton<PlayroomCustomizationModeController>.Instance.RefreshCustomizationMode(PlayroomCustomizationMode.None);
					}
					if (m_IsTop)
					{
						m_HintController.SelectItemTop.Disable();
					}
					else
					{
						m_HintController.SelectItemBot.Disable();
					}
				}
			}
			else
			{
				Logging.Log("Locked, wait till I finished loading...");
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

		private void SetGrayedOutAppropriately()
		{
			SetLocked(!WholeGameShopHelpers.IsItemUnlocked(m_Feature));
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

		private void SetTickAppropriately()
		{
			InstanceRegister instanceRegister = (InstanceRegister)m_TargetRoot.GetComponent("InstanceRegister");
			if (instanceRegister.AlreadyRegistered(m_TargetAssetBundle))
			{
				base.gameObject.GetChildGameObject("GUI_Tick").SetActive(true);
			}
		}

		public IEnumerator SelectFeature()
		{
			DeselectOtherFeatures();
			base.gameObject.GetChildGameObject("GUI_Tick").SetActive(true);
			yield return StartCoroutine(AddItemToScene());
			SavePreference();
			m_AmSwitchingPlayroomFeature = false;
		}

		private void SavePreference()
		{
			BabyInstance babyInstance = (BabyInstance)Object.FindObjectOfType(typeof(BabyInstance));
			if (!(babyInstance == null))
			{
				FurbyBaby targetFurbyBaby = babyInstance.GetTargetFurbyBaby();
				if (m_TargetRoot.name.EndsWith("WallArt"))
				{
					targetFurbyBaby.PlayroomCustomizations.WallArt = m_Feature.GetName();
				}
				else if (m_TargetRoot.name.EndsWith("LightFixtures"))
				{
					targetFurbyBaby.PlayroomCustomizations.LightFixture = m_Feature.GetName();
				}
				else if (m_TargetRoot.name.EndsWith("Props"))
				{
					targetFurbyBaby.PlayroomCustomizations.Prop = m_Feature.GetName();
				}
				else if (m_TargetRoot.name.EndsWith("Rugs"))
				{
					targetFurbyBaby.PlayroomCustomizations.Rug = m_Feature.GetName();
				}
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		public void DeselectOtherFeatures()
		{
			foreach (Transform item in base.transform.parent.gameObject.transform)
			{
				GameObject childGameObject = item.gameObject.GetChildGameObject("GUI_Tick");
				if ((bool)childGameObject)
				{
					childGameObject.SetActive(false);
				}
			}
		}

		private IEnumerator AddItemToScene()
		{
			InstanceRegister instanceRegister = (InstanceRegister)m_TargetRoot.GetComponent("InstanceRegister");
			if (!instanceRegister.AlreadyRegistered(m_TargetAssetBundle))
			{
				instanceRegister.DeregisterInstance();
				yield return StartCoroutine(LoadCustomisation());
				PutItemIntoCorrectLayer();
				instanceRegister.RegisterInstance(m_InstancedItem, m_TargetAssetBundle);
			}
		}

		private IEnumerator LoadCustomisation()
		{
			AssetBundleHelpers.AssetBundleLoad itemResult = new AssetBundleHelpers.AssetBundleLoad();
			yield return StartCoroutine(AssetBundleHelpers.Load("Playroom/" + m_TargetAssetBundle, true, itemResult, base.gameObject, typeof(GameObject), true));
			InstanceItem((GameObject)itemResult.m_object);
		}

		private GameObject InstanceItem(GameObject prefab)
		{
			m_InstancedItem = Object.Instantiate(prefab) as GameObject;
			m_InstancedItem.name = prefab.name;
			m_InstancedItem.SetActive(true);
			m_InstancedItem.transform.parent = m_TargetRoot.transform;
			m_InstancedItem.transform.Rotate(new Vector3(0f, 180f, 0f));
			return m_InstancedItem;
		}

		private void PutItemIntoCorrectLayer()
		{
			m_InstancedItem.layer = m_TargetRoot.layer;
			Transform[] componentsInChildren = m_InstancedItem.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.layer = m_TargetRoot.layer;
			}
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_Feature.GetName());
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_Feature);
		}

		public override bool ShouldUseAfterPurchase()
		{
			return true;
		}

		public override int GetFurbucksCost()
		{
			return m_Feature.GetCost();
		}
	}
}
